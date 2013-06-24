// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeavinsSoftware.Collection.SQLite;
using System.Data.SQLite;
using System.Data;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{
    sealed class RequireIssue : Migration
    {
        protected override void Up(SQLiteConnection connection)
        {
            IList<long> comicIds = new List<long>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT cb.comicid, i.issueid " +
                "FROM ComicBooks cb LEFT OUTER JOIN " +
                "ComicBookIssues i " +
                "ON cb.comicid = i.issueid;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(reader["issueid"].ToString()))
                        {
                            comicIds.Add(long.Parse(reader["comicid"].ToString()));
                        }
                    }
                }
            };

            foreach (long comicId in comicIds)
            {
                // Add a blank issue for each ComicBook without an issue
                SQL.InsertInto("ComicBookIssues")
                    .Column("comicid", comicId)
                    .Column("issue", "FILL_ME_IN")
                    .Column("listtype", ItemListType.Have)
                    .Column("issuetype", VolumeType.Issue)
                    .Column("distribution", DistributionType.Physical)
                    .ExecuteWith(connection);
            }
        }

        public override long SchemaVersion
        {
            get { return 3; }
        }
    }
}
