﻿// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{
    /// <summary>
    /// The only migration that should be run for new users.
    /// </summary>
    sealed class MigrationZero : Migration
    {
        protected override void Up(SQLiteConnection connection)
        {
            SQL.CreateTable("Categories")
                .Column("categoryid", "integer", "PRIMARY KEY")
                .Column("name", "text", "COLLATE NOCASE")
                .Column("type", "integer")
                .Column("code", "text")
                .ExecuteWith(connection);

            SQL.CreateTable("Products")
                .Column("productid", "integer", "PRIMARY KEY")
                .Column("name", "text", "COLLATE NOCASE")
                .Column("notes", "text", "COLLATE NOCASE")
                .Column("categoryid", "integer")
                .Column("quantity", "integer")
                .Column("listtype", "integer")
                .ForeignKey("FOREIGN KEY(categoryid) REFERENCES Categories(categoryid)")
                .ExecuteWith(connection);

            SQL.CreateTable("VideoGames")
                .Column("gameid", "integer", "PRIMARY KEY")
                .Column("name", "text", "COLLATE NOCASE")
                .Column("notes", "text", "COLLATE NOCASE")
                .Column("distribution", "integer")
                .Column("condition", "text", "COLLATE NOCASE")
                .Column("contentprovider", "text", "COLLATE NOCASE")
                .Column("categoryid", "integer")
                .Column("listtype", "integer")
                .ForeignKey("FOREIGN KEY(categoryid) REFERENCES Categories(categoryid)")
                .ExecuteWith(connection);

            SQL.CreateTable("ComicBooks")
                .Column("comicid", "integer", "PRIMARY KEY")
                .Column("name", "text", "COLLATE NOCASE")
                .Column("notes", "text", "COLLATE NOCASE")
                .Column("categoryid", "integer")
                .ForeignKey("FOREIGN KEY(categoryid) REFERENCES Categories(categoryid)")
                .ExecuteWith(connection);

            SQL.CreateTable("ComicBookIssues")
                .Column("issueid", "integer", "PRIMARY KEY")
                .Column("issue", "text", "COLLATE NOCASE")
                .Column("cover", "text", "COLLATE NOCASE")
                .Column("name", "text", "COLLATE NOCASE")
                .Column("distribution", "integer")
                .Column("condition", "text", "COLLATE NOCASE")
                .Column("issuetype", "integer")
                .Column("notes", "text", "COLLATE NOCASE")
                .Column("comicid", "integer")
                .Column("listtype", "integer")
                .ForeignKey("FOREIGN KEY(comicid) REFERENCES ComicBooks(comicid)")
                .ExecuteWith(connection);
            
            SQL.CreateTable("KeyValues")
                .Column("k", "Text", "Primary Key")
                .Column("v", "Text")
                .ExecuteWith(connection);
        }

        public override long SchemaVersion
        {
            get { return long.MaxValue; }
        }
    }
}
