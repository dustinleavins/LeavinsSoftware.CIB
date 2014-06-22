using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Program.Resources
{
    public interface IItemSpecificResources<TItem> : IItemSpecificResources where TItem : Item
    {
    }
}
