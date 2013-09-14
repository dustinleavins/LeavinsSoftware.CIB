﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class ModelSearchOptionsBuilder
    {
        public long ItemsPerPage { get; set; }

        public ItemCategory ItemCategory { get; set; }

        public ItemListType ListType { get; set; }

        public bool AllListTypes { get; set; }

        public string SearchText { get; set; }

        public ModelSearchOptions Build()
        {
            return new ModelSearchOptions(
                itemsPerPage: ItemsPerPage,
                itemCategory: ItemCategory,
                listType: ListType,
                allListTypes: AllListTypes,
                searchText: SearchText);
        }
    }
}