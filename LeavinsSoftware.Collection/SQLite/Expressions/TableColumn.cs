﻿using System.Globalization;
// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    public sealed class TableColumn : ISqlExpression
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Constraint { get; set; }

        public string ToCommandText()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Name, Type, Constraint).Trim();
        }
    }
}
