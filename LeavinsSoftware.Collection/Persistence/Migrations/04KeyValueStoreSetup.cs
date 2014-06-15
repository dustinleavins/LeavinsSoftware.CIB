// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Data.SQLite;
using LeavinsSoftware.Collection.SQLite;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{

    sealed class KeyValueStoreSetup : Migration
    {
        protected override void Up(SQLiteConnection connection)
        {
            SQL.CreateTable("KeyValues")
                .Column("k", "Integer", "Primary Key")
                .Column("v", "Text")
                .ExecuteWith(connection);
        }
        
        public override long SchemaVersion
        {
            get
            {
                return 4;
            }
        }
    }
}
