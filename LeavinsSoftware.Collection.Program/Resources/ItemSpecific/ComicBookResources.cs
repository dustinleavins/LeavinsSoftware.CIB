using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Program.Resources.ItemSpecific
{
    internal class ComicBookResources : IItemSpecificResources<ComicBookSeries>
    {
        public string AddSubCategory
        {
            get { return InterfaceResources.AddSubCategory_ComicBook; }
        }

        public string Name
        {
            get { return InterfaceResources.Common_ComicBooks; }
        }
    }
}
