using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Program.Resources.ItemSpecific
{
    internal class VideoGameResources : IItemSpecificResources<VideoGame>
    {
        public string AddSubCategory
        {
            get { return InterfaceResources.AddSubCategory_VideoGame; }
        }

        public string Name
        {
            get { return InterfaceResources.Common_VideoGames; }
        }
    }
}
