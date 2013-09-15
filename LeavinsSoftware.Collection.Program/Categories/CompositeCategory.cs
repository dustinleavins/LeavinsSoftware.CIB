using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public sealed class CompositeCategory : CategoryBase
    {
        public CompositeCategory(IEnumerable<DefaultCategory> categories)
        {
            innerCategories = categories;
        }

        public override bool IsComposite
        {
            get { return true; }
        }

        public override IEnumerable<DefaultCategory> Categories
        {
            get
            {
                return innerCategories;
            }
        }

        public override ItemCategory ToItemCategory()
        {
            throw new NotSupportedException();
        }

        private IEnumerable<DefaultCategory> innerCategories;
    }
}
