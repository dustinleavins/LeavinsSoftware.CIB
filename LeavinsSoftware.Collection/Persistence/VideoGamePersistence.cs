// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class VideoGamePersistence : PersistenceBase<VideoGame>, IVideoGamePersistence
    {
        public VideoGamePersistence(string dataPath, string initialProfileName)
        {
            string fullPath = Path.Combine(dataPath, initialProfileName, "collection.db");
            ConnectionString = string.Format("Data Source=|DataDirectory|{0}",
                fullPath);
        }

        public string ConnectionString { get; private set; }

        protected override VideoGame CreateBase(VideoGame item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.InsertInto("VideoGames")
                    .Column("name", item.Name)
                    .Column("notes", item.Notes)
                    .Column("categoryid", item.System.Id)
                    .Column("distribution", item.DistributionType)
                    .Column("condition", item.Condition)
                    .Column("contentprovider", item.ContentProvider)
                    .Column("listtype", item.ListType)
                    .ExecuteWith(connection);

                item.Id = connection.LastInsertRowId;
                connection.Close();
            }

            return item;
        }

        protected override VideoGame RetrieveBase(long id)
        {
            VideoGame targetGame = null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                long categoryId = 0;

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * " +
                        "FROM VideoGames " +
                        "WHERE rowid = @id";

                    cmd.Parameters.Add(new SQLiteParameter("id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            targetGame = ReaderToGame(reader);

                            categoryId = long.Parse(reader["categoryid"].ToString());
                        }
                    }
                }

                if (targetGame != null)
                {
                    targetGame.System = GetCategory(categoryId, connection);
                }

                connection.Close();
            }

            return targetGame;
        }

        protected override VideoGame UpdateBase(VideoGame item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.Update("VideoGames")
                    .Set("name", item.Name)
                    .Set("notes", item.Notes)
                    .Set("categoryid", item.System.Id)
                    .Set("distribution", item.DistributionType)
                    .Set("condition", item.Condition)
                    .Set("contentprovider", item.ContentProvider)
                    .Set("listtype", item.ListType)
                    .WhereEquals("gameid", item.Id)
                    .ExecuteWith(connection);

                connection.Close();
            }

            return item;
        }

        protected override void DeleteBase(VideoGame item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.DeleteFrom("VideoGames")
                    .WhereEquals("gameid", item.Id)
                    .ExecuteWith(connection);

                connection.Close();
            }
        }

        public List<VideoGame> Page(ModelSearchOptions options, long pageNumber)
        {
            List<VideoGame> page = new List<VideoGame>();

            Dictionary<long, long> categoryIdDict =
                new Dictionary<long, long>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * " +
                        "FROM VideoGames " +
                        "WHERE name LIKE @name ";

                    string searchParamValue = "%";

                    if (!string.IsNullOrEmpty(options.SearchText))
                    {
                        searchParamValue = string.Format(CultureInfo.InvariantCulture,
                            "%{0}%", options.SearchText);
                    }

                    cmd.Parameters.Add(new SQLiteParameter("name", searchParamValue));

                    if (options.ItemCategory != null)
                    {
                        cmd.CommandText += "AND categoryid = @categoryid ";
                        cmd.Parameters.Add(new SQLiteParameter("@categoryid",
                            options.ItemCategory.Id));

                        if (!options.AllListTypes)
                        {
                            cmd.CommandText += "AND listtype = @listtype ";
                            cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                        }
                    }
                    else if (!options.AllListTypes)
                    {
                        cmd.CommandText += "AND listtype = @listtype ";
                        cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                    }

                    cmd.CommandText += "ORDER BY name LIMIT @limit OFFSET @offset;";
                    cmd.Parameters.Add(new SQLiteParameter("@limit", options.ItemsPerPage));
                    cmd.Parameters.Add(new SQLiteParameter("@offset", options.ItemsPerPage * pageNumber));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VideoGame game = ReaderToGame(reader);

                            categoryIdDict[game.Id] =
                                long.Parse(reader["categoryid"].ToString());

                            page.Add(game);
                        }
                    }
                }

                if (options.ItemCategory != null)
                {
                    foreach (VideoGame item in page)
                    {
                        item.System = options.ItemCategory;
                    }
                }
                else
                {
                    foreach (VideoGame item in page)
                    {
                        long categoryId = categoryIdDict[item.Id];
                        item.System = GetCategory(categoryId, connection);
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
                        "FROM VideoGames " +
                        "WHERE name LIKE @name ";

                    string searchParamValue = "%";

                    if (!string.IsNullOrEmpty(options.SearchText))
                    {
                        searchParamValue = string.Format(CultureInfo.InvariantCulture,
                            "%{0}%", options.SearchText);
                    }

                    cmd.Parameters.Add(new SQLiteParameter("name", searchParamValue));

                    if (options.ItemCategory != null)
                    {
                        cmd.CommandText += "AND categoryid = @categoryid ";
                        cmd.Parameters.Add(new SQLiteParameter("@categoryid",
                            options.ItemCategory.Id));

                        if (!options.AllListTypes)
                        {
                            cmd.CommandText += "AND listtype = @listtype";
                            cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                        }
                    }
                    else if (!options.AllListTypes)
                    {
                        cmd.CommandText += "AND listtype = @listtype";
                        cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                    }

                    cmd.CommandText += ";";

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var countNullable = reader[0] as long?;
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
                        targetCategory.Id = long.Parse(reader["categoryid"].ToString());
                        targetCategory.Name = reader["name"].ToString();
                        targetCategory.Code = reader["code"].ToString();
                        targetCategory.CategoryType = (ItemCategoryType)int.Parse(reader["type"].ToString());
                    }
                }
            }

            return targetCategory;
        }

        private static VideoGame ReaderToGame(SQLiteDataReader reader)
        {
            VideoGame targetGame = new VideoGame();

            targetGame.Id = long.Parse(reader["gameid"].ToString());
            targetGame.Name = reader["name"].ToString();
            targetGame.Notes = reader["notes"].ToString();
            targetGame.DistributionType = (DistributionType)int.Parse(reader["distribution"].ToString());
            targetGame.Condition = reader["condition"].ToString();
            targetGame.ContentProvider = reader["contentprovider"].ToString();
            targetGame.ListType = (ItemListType)int.Parse(reader["listtype"].ToString());

            return targetGame;
        }
    }
}
