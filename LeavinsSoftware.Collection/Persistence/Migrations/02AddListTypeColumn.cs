// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{
    sealed class AddListTypeColumn : Migration
    {
        protected override void Up(System.Data.SQLite.SQLiteConnection connection)
        {
            SQL.AlterTable("Products")
                .AddColumn("listtype", "integer")
                .ExecuteWith(connection);

            SQL.AlterTable("VideoGames")
                .AddColumn("listtype", "integer")
                .ExecuteWith(connection);

            SQL.AlterTable("ComicBookIssues")
                .AddColumn("listtype", "integer")
                .ExecuteWith(connection);

            SQL.Update("Products")
                .Set("listtype", ItemListType.Have)
                .ExecuteWith(connection);

            SQL.Update("VideoGames")
                .Set("listtype", ItemListType.Have)
                .ExecuteWith(connection);

            SQL.Update("ComicBookIssues")
                .Set("listtype", ItemListType.Have)
                .ExecuteWith(connection);
        }

        public override long SchemaVersion
        {
            get { return 2; }
        }
    }
}
