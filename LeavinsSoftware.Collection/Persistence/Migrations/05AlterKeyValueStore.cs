// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Data;
using System.Data.SQLite;
using LeavinsSoftware.Collection.SQLite;

namespace LeavinsSoftware.Collection.Persistence.Migrations
{
    sealed class AlterKeyValueStore : Migration
    {
        protected override void Up(SQLiteConnection connection)
        {
            /* Drop Existing Table */
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DROP TABLE KeyValues;";
                cmd.ExecuteNonQuery();
            }
            
            SQL.CreateTable("KeyValues")
                .Column("k", "Text", "Primary Key")
                .Column("v", "Text")
                .ExecuteWith(connection);
        }
        
        public override long SchemaVersion
        {
            get
            {
                return 5;
            }
        }
    }
}
