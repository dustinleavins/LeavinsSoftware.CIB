// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    public sealed class Definition : ISqlExpression
    {
        public Definition(string definitionValue)
        {
            Value = definitionValue;
        }
        public string Value { get; set; }

        public string ToCommandText()
        {
            return Value;
        }
    }
}
