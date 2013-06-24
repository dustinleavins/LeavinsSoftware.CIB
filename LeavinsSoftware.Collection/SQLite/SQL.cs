// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Collections.Generic;
using LeavinsSoftware.Collection.SQLite.Expressions;
using LeavinsSoftware.Collection.SQLite.Conversion;

namespace LeavinsSoftware.Collection.SQLite
{
    /// <summary>
    /// Provides a builder-style interface for building SQLite statements
    /// and converting data to/from a preferred DB-friendly format.
    /// </summary>
    public static class SQL
    {
        private const string DbDataTimeFormat = "yyyyMMdd-HH:mm:ss";

        public static ToDatabaseFormatHelper ToDatabaseFormat()
        {
            return ToDatabaseFormatHelper.Instance;
        }

        public static FromDatabaseFormatHelper FromDatabaseFormat()
        {
            return FromDatabaseFormatHelper.Instance;
        }

        /// <summary>
        /// SQL statement builder - create a table with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CreateTableExpression CreateTable(string name)
        {
            return new CreateTableExpression(name);
        }

        /// <summary>
        /// SQL statement builder - insert data into the table with the given name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static InsertExpression InsertInto(string tableName)
        {
            return new InsertExpression(tableName);
        }

        /// <summary>
        /// SQL statement builder - update data in the table with the given name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static UpdateExpression Update(string tableName)
        {
            return new UpdateExpression(tableName);
        }

        /// <summary>
        /// SQL statement builder - alter the table with the given name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static AlterTableExpression AlterTable(string tableName)
        {
            return new AlterTableExpression(tableName);
        }

        /// <summary>
        /// SQL statement builder - delete a row from the table with
        /// the given name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DeleteFromExpression DeleteFrom(string tableName)
        {
            return new DeleteFromExpression(tableName);
        }
    }
}
