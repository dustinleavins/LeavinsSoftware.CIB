// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Text;

namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    /// <summary>
    /// Update Expression.
    /// </summary>
    public sealed class UpdateExpression : ISqlExpression
    {
        public UpdateExpression(string tableName)
        {
            TableName = tableName;
            Values = new List<string>();
            ParameterMapping = new Dictionary<string, object>();
        }

        public string TableName { get; private set; }

        public string WhereClause { get; set; }

        public string ToCommandText()
        {
            var builder = new StringBuilder();
            builder.Append("UPDATE ")
                .Append(TableName)
                .Append(" SET ");

            bool firstRow = true;

            foreach (var v in Values)
            {
                if (!firstRow)
                {
                    builder.Append(",");
                }

                builder.Append(string.Format(CultureInfo.InvariantCulture, "{0} = @{0}", v));
                firstRow = false;
            }

            if (!string.IsNullOrEmpty(WhereClause))
            {
                builder.Append(" WHERE ")
                    .Append(WhereClause);
            }

            return builder.Append(";").ToString();
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

        public UpdateExpression Set(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Values.Add("NULL");
            }
            else
            {
                Values.Add(value);
            }

            return this;
        }

        public UpdateExpression Set(string parameterName, object parameterValue)
        {
            Values.Add(parameterName);
            ParameterMapping.Add("@" + parameterName, parameterValue);
            return this;
        }

        public UpdateExpression WhereEquals(string parameterName, object parameterValue)
        {
            WhereClause = String.Format(CultureInfo.InvariantCulture,
                                        "{0} = {1}",
                                        parameterName,
                                        "@" + parameterName);
            ParameterMapping.Add("@" + parameterName, parameterValue);

            return this;
        }

        private ICollection<string> Values { get; set; }
        private IDictionary<string, object> ParameterMapping { get; set; }
    }
}
