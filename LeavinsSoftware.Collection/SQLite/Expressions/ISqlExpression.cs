// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
namespace LeavinsSoftware.Collection.SQLite.Expressions
{
    /// <summary>
    /// Interface for SQL expression classes.
    /// </summary>
    public interface ISqlExpression
    {
        /// <summary>
        /// Gets the command text of this SQL expression.
        /// </summary>
        /// <returns></returns>
        string ToCommandText();
    }
}
