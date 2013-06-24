// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    /// <summary>
    /// Insert Expression
    /// </summary>
    public sealed class InsertExpression : ISqlExpression
    {
        public InsertExpression(string tableName)
        {
            TableName = tableName;
            Values = new List<string>();
            ParameterMapping = new Dictionary<string, object>();
        }

        public string TableName { get; private set; }

        public string ToCommandText()
        {
            var builder = new StringBuilder();
            builder.Append(InsertWithConflictText())
                .Append(TableName)
                .Append("(");

            bool firstRow = true;

            foreach (var v in Values)
            {
                if (!firstRow)
                {
                    builder.Append(",");
                }

                builder.Append(v);
                firstRow = false;
            }

            builder.Append(") VALUES (");

            firstRow = true;

            foreach (var v in Values)
            {
                if (!firstRow)
                {
                    builder.Append(",");
                }

                builder.Append("@").Append(v);
                firstRow = false;
            }

            builder.Append(")");

            return builder.ToString();
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

        public InsertExpression Column(string columnName, object parameterValue)
        {
            Values.Add(columnName);
            ParameterMapping.Add("@" + columnName, parameterValue);
            return this;
        }

        public InsertExpression Or(string conflictResolutionAlgorithm)
        {
            _conflictText = conflictResolutionAlgorithm;
            return this;
        }

        private string InsertWithConflictText()
        {
            if (string.IsNullOrWhiteSpace(_conflictText))
            {
                return "INSERT INTO ";
            }
            else
            {
                return "INSERT OR " + _conflictText + " INTO ";
            }
        }

        private string _conflictText;
        private ICollection<string> Values { get; set; }
        private IDictionary<string, object> ParameterMapping { get; set; }
    }
}
