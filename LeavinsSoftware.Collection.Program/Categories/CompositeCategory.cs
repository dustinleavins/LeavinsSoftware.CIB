// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public sealed class CompositeCategory : IDefaultCategory
    {
        public CompositeCategory(IEnumerable<DefaultCategory> categories)
        {
            innerCategories = categories;
        }

        public string Name
        {
            get;
            set;
        }

        public ItemCategoryType CategoryType
        {
            get;
            set;
        }
        
        public string Code
        {
            get;
            set;
        }

        public bool IsComposite
        {
            get { return true; }
        }

        public IEnumerable<DefaultCategory> Categories
        {
            get
            {
                return innerCategories;
            }
        }

        private readonly IEnumerable<DefaultCategory> innerCategories;
    }
}
