// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;

namespace LeavinsSoftware.Collection.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class ItemCategoryTypeAttribute : Attribute
    {
        public ItemCategoryTypeAttribute(ItemCategoryType categoryType)
        {
            CategoryType = categoryType;
        }
        
        public ItemCategoryType CategoryType { get; private set; }
    }
}
