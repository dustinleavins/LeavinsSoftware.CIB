// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    public sealed class DeleteFromExpression : ISqlExpression
    {
        public DeleteFromExpression(string tableName)
        {
            TableName = tableName;
            ParameterMapping = new Dictionary<string, object>();
        }

        public DeleteFromExpression WhereEquals(string parameterName, object parameterValue)
        {
            WhereClause = String.Format("{0} = {1}",
                                        parameterName,
                                        "@" + parameterName);

            ParameterMapping.Add("@" + parameterName, parameterValue);

            return this;
        }

        public void ExecuteWith(SQLiteConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = ToCommandText();
                foreach (var pair in ParameterMapping)
                {
                    var param = new SQLiteParameter(pair.Key, pair.Value);
                    cmd.Parameters.Add(param);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public string ToCommandText()
        {
            var builder = new StringBuilder();

            builder.Append("DELETE FROM ")
                .Append(TableName)
                .Append(" WHERE ")
                .Append(WhereClause)
                .Append(";");

            return builder.ToString();
        }

        public string TableName { get; private set; }
        public string WhereClause { get; private set; }
        private IDictionary<string, object> ParameterMapping { get; set; }
    }
}
