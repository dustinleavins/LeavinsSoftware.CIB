// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Attributes
{
    /// <summary>
    /// Marker attribute for pages that show multiple items in a collection.
    /// </summary>
    /// <remarks>
    /// This attribute is primarily intended for navigational purposes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CollectionPageAttribute : Attribute
    {
        public CollectionPageAttribute(ItemCategoryType categoryType)
        {
            CategoryType = categoryType;
        }

        /// <summary>
        /// <see cref="ItemCategoryType"/> of the collection being shown
        /// on this page.
        /// </summary>
        public ItemCategoryType CategoryType
        {
            get;
            private set;
        }
    }
}
