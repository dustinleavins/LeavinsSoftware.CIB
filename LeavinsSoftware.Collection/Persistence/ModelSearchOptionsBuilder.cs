// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Builder for <see cref="ModelSearchOptions"/>.
    /// </summary>
    public sealed class ModelSearchOptionsBuilder
    {
        public long ItemsPerPage { get; set; }

        public ItemCategory ItemCategory { get; set; }

        public ItemListType? ListType { get; set; }

        public string SearchText { get; set; }

        public ModelSearchOptions Build()
        {
            return new ModelSearchOptions(
                itemsPerPage: ItemsPerPage,
                itemCategory: ItemCategory,
                listType: ListType,
                searchText: SearchText);
        }
    }
}
