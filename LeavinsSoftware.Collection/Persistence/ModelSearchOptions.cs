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
    public struct ModelSearchOptions
    {
        public ModelSearchOptions(long itemsPerPage,
            ItemCategory itemCategory,
            ItemListType listType,
            bool allListTypes,
            string searchText)
        {
            ItemsPerPage = itemsPerPage;
            ItemCategory = itemCategory;
            ListType = listType;
            AllListTypes = allListTypes;
            SearchText = searchText;
        }

        public readonly long ItemsPerPage;

        public readonly ItemCategory ItemCategory;

        public readonly ItemListType ListType;

        public readonly bool AllListTypes;

        public readonly string SearchText;
    }
}
