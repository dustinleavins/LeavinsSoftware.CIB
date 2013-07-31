// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CollectionPageAttribute : Attribute
    {
        public CollectionPageAttribute(ItemCategoryType categoryType)
        {
            CategoryType = categoryType;
        }

        public ItemCategoryType CategoryType
        {
            get;
            private set;
        }
    }
}
