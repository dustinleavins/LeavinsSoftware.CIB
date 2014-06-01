// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    /// <summary>
    /// Create Table Expression
    /// </summary>
    public sealed class CreateTableExpression : ISqlExpression
    {
        public CreateTableExpression(string name)
        {
            TableName = name;
            Columns = new List<ISqlExpression>();
        }

        public CreateTableExpression ForeignKey(string definition)
        {
            Columns.Add(new Definition(definition));
            return this;
        }

        public CreateTableExpression Column(string name, string type)
        {
            Columns.Add(new TableColumn { Name = name, Type = type });
            return this;
        }

        public CreateTableExpression Column(string name, string type, string constraint)
        {
            Columns.Add(new TableColumn
            {
                Name = name,
                Type = type,
                Constraint = constraint
            });

            return this;
        }

        public void ExecuteWith(SQLiteConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = ToCommandText();
                cmd.ExecuteNonQuery();
            }
        }

        public string ToCommandText()
        {
            var builder = new StringBuilder();

            builder.Append("CREATE TABLE IF NOT EXISTS ")
                .Append(TableName)
                .Append(" (");

            bool firstColumn = true;

            foreach (var column in Columns)
            {
                if (!firstColumn)
                {
                    builder.Append(", ");
                }

                builder.Append(column.ToCommandText());
                firstColumn = false;
            }

            builder.Append(");");

            return builder.ToString();
        }

        public string TableName { get; private set; }
        private ICollection<ISqlExpression> Columns { get; set; }
    }
}
