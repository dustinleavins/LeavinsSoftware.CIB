using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    /// <summary>
    /// Interface for a default category or a category group.
    /// </summary>
    /// <remarks>
    /// Why does <see cref="ICategory"/> represent a single category and
    /// a group? It allows back-end code to build a menu of categories
    /// where categories do not have to belong to a group.
    /// </remarks>
    public interface ICategory
    {
        string Name { get; set; }

        ItemCategoryType CategoryType { get; set; }

        /// <summary>
        /// Does this instance contain multiple categories?
        /// </summary>
        bool IsComposite { get; }

        /// <summary>
        /// Returns an enumeration of nested <see cref="DefaultCategory"/>
        /// instances.
        /// </summary>
        /// <remarks>
        /// This has type <see cref="DefaultCategory"/> to limit category nesting.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Thrown if this instance does not support composition.
        /// </exception>
        IEnumerable<DefaultCategory> Categories { get; }

        /// <summary>
        /// Returns a new instance of <see cref="ItemCategory"/> that contains
        /// the same data as this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if this instance does not support conversion to
        /// <see cref="ItemCategory"/>.
        /// </exception>
        ItemCategory ToItemCategory();
    }
}
