using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public sealed class DefaultCategory : CategoryBase
    {
        public string Code { get; set; }

        public override bool IsComposite
        {
            get { return false; }
        }

        public override IEnumerable<DefaultCategory> Categories
        {
            get { throw new NotSupportedException(); }
        }

        public override ItemCategory ToItemCategory()
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
