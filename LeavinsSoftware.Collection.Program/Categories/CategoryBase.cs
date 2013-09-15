using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public abstract class CategoryBase
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

        public abstract bool IsComposite
        {
            get;
        }

        public abstract IEnumerable<DefaultCategory> Categories
        {
            get;
        }

        public abstract ItemCategory ToItemCategory();
    }
}
