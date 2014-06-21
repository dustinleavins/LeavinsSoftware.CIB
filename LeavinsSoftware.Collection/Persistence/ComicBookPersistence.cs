// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Globalization;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class ComicBookPersistence : PersistenceBase<ComicBookSeries>, IComicBookPersistence
    {
        public ComicBookPersistence(DirectoryInfo dataDir, Profile initialProfile)
        {
            string fullPath = Path.Combine(dataDir.FullName, initialProfile.Name, "collection.db");
            ConnectionString = string.Format(CultureInfo.InvariantCulture,
                "Data Source=|DataDirectory|{0}",
                fullPath);
        }

        public string ConnectionString { get; private set; }

        protected override ComicBookSeries CreateBase(ComicBookSeries item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.InsertInto("ComicBooks")
                    .Column("name", item.Name)
                    .Column("notes", item.Notes)
                    .Column("categoryid", item.Publisher.Id)
                    .ExecuteWith(connection);

                item.Id = connection.LastInsertRowId;

                foreach (ComicBookSeriesEntry issue in item.Entries)
                {
                    CreateIssue(issue, item.Id, connection);
                }

                connection.Close();
            }

            return item;
        }

        protected override ComicBookSeries RetrieveBase(long id)
        {
            ComicBookSeries targetBook = null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                long categoryId = 0;

                // Retrieve main book
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * " +
                        "FROM ComicBooks " +
                        "WHERE rowid = @id";

                    cmd.Parameters.Add(new SQLiteParameter("id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            targetBook = ReaderToComicBook(reader);

                            categoryId = long.Parse(reader["categoryid"].ToString(),
                                CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (targetBook != null)
                {
                    // Retrieve issues
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * " +
                            "FROM ComicBookIssues " +
                            "WHERE comicid = @comicid " +
                            "ORDER BY name;";

                        cmd.Parameters.Add(new SQLiteParameter("comicid", id));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                targetBook.Entries.Add(ReaderToIssue(reader));
                            }
                        }
                    }

                    // Retrieve category
                    targetBook.Publisher = GetCategory(categoryId, connection);
                }

                connection.Close();
            }

            return targetBook;
        }

        protected override ComicBookSeries UpdateBase(ComicBookSeries item)
        {
            ComicBookSeries originalBook = Retrieve(item.Id);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.Update("ComicBooks")
                    .Set("name", item.Name)
                    .Set("notes", item.Notes)
                    .Set("categoryid", item.Publisher.Id)
                    .WhereEquals("comicid", item.Id)
                    .ExecuteWith(connection);

                foreach (ComicBookSeriesEntry issue in item.Entries)
                {
                    if (issue.IsNew)
                    {
                    	CreateIssue(issue, item.Id, connection);      
                    }
                    else
                    {
                       UpdateIssue(issue, connection);
                    }
                }

                IEnumerable<long> removedIssueIds = originalBook
                    .Entries
                    .Select(i => i.Id)
                    .Except(item.Entries.Select(i => i.Id));

                foreach (long issueId in removedIssueIds)
                {
                    DeleteIssueById(issueId, connection);
                }

                connection.Close();
            }

            return item;
        }

        protected override void DeleteBase(ComicBookSeries item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.DeleteFrom("ComicBooks")
                    .WhereEquals("comicid", item.Id)
                    .ExecuteWith(connection);

                // Delete issues
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM ComicBookIssues " +
                        "WHERE comicid = @comicid;";

                    cmd.Parameters.Add(new SQLiteParameter("comicid", item.Id));
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }

            // TODO: Trigger changed event
        }

        public List<ComicBookSeries> Page(ModelSearchOptions options, long pageNumber)
        {
            List<ComicBookSeries> page = new List<ComicBookSeries>();

            Dictionary<long, long> categoryIdDict =
                new Dictionary<long, long>();
            
            String issueCountQuery;
            if (options.ListType.HasValue)
            {
                issueCountQuery = "SELECT comicId, count(issueId) as count " +
                    "FROM ComicBookIssues " +
                    "WHERE listtype = @listtype " +
                    "GROUP BY comicId ";
                    
            }
            else
            {
                issueCountQuery = "SELECT comicId, count(issueId) as count " +
                    "FROM ComicBookIssues " +
                    "GROUP BY comicId ";
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT cb.comicid, cb.name, " +
                        "cb.notes, cb.categoryid, iCount.count " +
                        "FROM ComicBooks cb " +
                        "INNER JOIN Categories cat ON cb.categoryId = cat.categoryId " +
                        "INNER JOIN " +
                        "( " +
                        issueCountQuery +
                        ") iCount ON iCount.comicId = cb.comicId " +
                        "WHERE cb.name LIKE @name ";

                    string searchParamValue = "%";

                    if (!string.IsNullOrEmpty(options.SearchText))
                    {
                        searchParamValue = string.Format(CultureInfo.InvariantCulture,
                            "%{0}%", options.SearchText);
                    }

                    cmd.Parameters.Add(new SQLiteParameter("name", searchParamValue));

                    if (options.ListType.HasValue)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                    }

                    if (options.ItemCategory != null)
                    {
                        cmd.CommandText += "AND cb.categoryid = @categoryid ";
                        cmd.Parameters.Add(new SQLiteParameter("@categoryid",
                            options.ItemCategory.Id));
                    }

                    cmd.CommandText += "ORDER BY cat.name, cb.name LIMIT @limit OFFSET @offset;";

                    cmd.Parameters.Add(new SQLiteParameter("@limit", options.ItemsPerPage));
                    cmd.Parameters.Add(new SQLiteParameter("@offset", options.ItemsPerPage * pageNumber));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComicBookSeries book = ReaderToComicBookSummary(reader);

                            categoryIdDict[book.Id] =
                                long.Parse(reader["categoryid"].ToString(),
                                CultureInfo.InvariantCulture);

                            page.Add(book);
                        }
                    }
                }

                if (options.ItemCategory != null)
                {
                    foreach (ComicBookSeries item in page)
                    {
                        item.Publisher = options.ItemCategory;
                    }
                }
                else
                {
                    foreach (ComicBookSeries item in page)
                    {
                        long categoryId = categoryIdDict[item.Id];
                        item.Publisher = GetCategory(categoryId, connection);
                    }
                }

                connection.Close();
            }

            return page;
        }

        public long TotalResults(ModelSearchOptions options)
        {
            long total = 0;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT COUNT(*) " +
                        "FROM (SELECT DISTINCT cb.comicid FROM ComicBooks cb, ComicBookIssues i " +
                        "WHERE cb.comicid = i.comicid " +
                        "AND cb.name LIKE @name ";

                    string searchParamValue = "%";

                    if (!string.IsNullOrEmpty(options.SearchText))
                    {
                        searchParamValue = string.Format(CultureInfo.InvariantCulture,
                            "%{0}%", options.SearchText);
                    }

                    cmd.Parameters.Add(new SQLiteParameter("name", searchParamValue));

                    if (options.ListType.HasValue)
                    {
                        cmd.CommandText += "AND i.listtype = @listtype ";
                        cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                    }

                    if (options.ItemCategory == null)
                    {
                        cmd.CommandText += ");";
                    }
                    else
                    {
                        cmd.CommandText += "AND categoryid = @categoryid);";
                        cmd.Parameters.Add(new SQLiteParameter("@categoryid",
                            options.ItemCategory.Id));
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var countNullable = (long?)reader[0];
                            total = countNullable.Value;
                        }
                    }
                }

                connection.Close();
            }

            return total;
        }

        private static ItemCategory GetCategory(long id, SQLiteConnection openConnection)
        {
            ItemCategory targetCategory = null;

            using (var cmd = openConnection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT categoryid, name, code, type " +
                    "FROM Categories " +
                    "WHERE rowid = @id";

                cmd.Parameters.Add(new SQLiteParameter("id", id));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        targetCategory = new ItemCategory();
                        targetCategory.Id = long.Parse(reader["categoryid"].ToString(),
                            CultureInfo.InvariantCulture);
                        targetCategory.Name = reader["name"].ToString();
                        targetCategory.Code = reader["code"].ToString();
                        targetCategory.CategoryType = (ItemCategoryType)int.Parse(reader["type"].ToString(),
                            CultureInfo.InvariantCulture);
                    }
                }
            }

            return targetCategory;
        }

        private static ComicBookSeries ReaderToComicBook(SQLiteDataReader reader)
        {
            ComicBookSeries targetBook = new ComicBookSeries();

            targetBook.Id = long.Parse(reader["comicid"].ToString(),
                CultureInfo.InvariantCulture);
            targetBook.Name = reader["name"].ToString();
            targetBook.Notes = reader["notes"].ToString();

            return targetBook;
        }

        private static ComicBookSeries ReaderToComicBookSummary(SQLiteDataReader reader)
        {
            long entriesCount = long.Parse(reader["count"].ToString(),
                CultureInfo.InvariantCulture);
            
            ComicBookSeries targetBook = ComicBookSeries.NewSummary(entriesCount);

            targetBook.Id = long.Parse(reader["comicid"].ToString(),
                CultureInfo.InvariantCulture);
            targetBook.Name = reader["name"].ToString();
            targetBook.Notes = reader["notes"].ToString();

            return targetBook;
        }

        private static ComicBookSeriesEntry ReaderToIssue(SQLiteDataReader reader)
        {
            ComicBookSeriesEntry targetBook = new ComicBookSeriesEntry();

            targetBook.Id = long.Parse(reader["issueid"].ToString(), CultureInfo.InvariantCulture);
            targetBook.Name = reader["name"].ToString();
            targetBook.Notes = reader["notes"].ToString();
            targetBook.Number = reader["issue"].ToString();
            targetBook.Cover = reader["cover"].ToString();
            targetBook.DistributionType = (DistributionType)int.Parse(reader["distribution"].ToString(), CultureInfo.InvariantCulture);
            targetBook.Condition = reader["condition"].ToString();
            targetBook.EntryType = (VolumeType)int.Parse(reader["issuetype"].ToString(), CultureInfo.InvariantCulture);
            targetBook.SeriesId = int.Parse(reader["comicid"].ToString(), CultureInfo.InvariantCulture);
            targetBook.ListType = (ItemListType)int.Parse(reader["listtype"].ToString(), CultureInfo.InvariantCulture);

            return targetBook;
        }

        private static void CreateIssue(ComicBookSeriesEntry issue, long comicId, SQLiteConnection openConnection)
        {
            issue.SeriesId = comicId;

            SQL.InsertInto("ComicBookIssues")
                .Column("issue", issue.Number)
                .Column("cover", issue.Cover)
                .Column("name", issue.Name)
                .Column("distribution", issue.DistributionType)
                .Column("condition", issue.Condition)
                .Column("issuetype", issue.EntryType)
                .Column("notes", issue.Notes)
                .Column("comicid", issue.SeriesId)
                .Column("listtype", issue.ListType)
                .ExecuteWith(openConnection);

            issue.Id = openConnection.LastInsertRowId;
        }

        private static void UpdateIssue(ComicBookSeriesEntry issue, SQLiteConnection openConnection)
        {
            SQL.Update("ComicBookIssues")
                .Set("issue", issue.Number)
                .Set("cover", issue.Cover)
                .Set("name", issue.Name)
                .Set("distribution", issue.DistributionType)
                .Set("condition", issue.Condition)
                .Set("issuetype", issue.EntryType)
                .Set("notes", issue.Notes)
                .Set("comicid", issue.SeriesId)
                .Set("listtype", issue.ListType)
                .WhereEquals("issueid", issue.Id)
                .ExecuteWith(openConnection);
        }

        private static void DeleteIssueById(long issueId, SQLiteConnection openConnection)
        {
            SQL.DeleteFrom("ComicBookIssues")
                .WhereEquals("issueid", issueId)
                .ExecuteWith(openConnection);
        }
    }
}
