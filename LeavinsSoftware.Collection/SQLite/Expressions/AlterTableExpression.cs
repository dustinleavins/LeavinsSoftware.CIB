// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    public sealed class AlterTableExpression : ISqlExpression
    {
        public AlterTableExpression(string name)
        {
            TableName = name;
        }

        public AlterTableExpression AddColumn(string name, string type)
        {
            Column = new TableColumn { Name = name, Type = type };
            return this;
        }

        public AlterTableExpression AddColumn(string name, string type, string constraint)
        {
            Column = new TableColumn
            {
                Name = name,
                Type = type,
                Constraint = constraint
            };

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

            builder.Append("ALTER TABLE ")
                .Append(TableName)
                .Append(" ADD COLUMN ")
                .Append(Column.ToCommandText())
                .Append(";");

            return builder.ToString();
        }

        public string TableName { get; private set; }
        private ISqlExpression Column { get; set; }
    }
}
