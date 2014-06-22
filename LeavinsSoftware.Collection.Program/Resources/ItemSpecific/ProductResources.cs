using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Program.Resources.ItemSpecific
{
    internal class ProductResources : IItemSpecificResources<Product>
    {
        public string AddSubCategory
        {
            get { return InterfaceResources.AddSubCategory_Product; }
        }

        public string Name
        {
            get { return InterfaceResources.Common_Products; }
        }
    }
}
