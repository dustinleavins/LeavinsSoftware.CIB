using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public sealed class DefaultCategory : ICategory
    {
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
            get { return false; }
        }

        public IEnumerable<DefaultCategory> Categories
        {
            get { throw new NotSupportedException(); }
        }

        public ItemCategory ToItemCategory()
        {
            return new ItemCategory()
            {
                Name = this.Name,
                CategoryType = this.CategoryType,
                Code = this.Code
            };
        }
    }
}
