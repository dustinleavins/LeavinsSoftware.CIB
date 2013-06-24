// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public interface ICategoryPersistence : IPersistence<ItemCategory>
    {
        ICollection<ItemCategory> RetrieveAll(ItemCategoryType type);

        long Count(ItemCategoryType type);

        bool Any(ItemCategoryType type);
    }
}
