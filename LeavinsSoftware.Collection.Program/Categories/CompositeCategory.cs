using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public sealed class CompositeCategory : ICategory
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

        public ItemCategory ToItemCategory()
        {
            throw new NotSupportedException();
        }

        private IEnumerable<DefaultCategory> innerCategories;
    }
}
