// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// The list that an item belongs to.
    /// </summary>
    public enum ItemListType
    {
        Have = 0,

        Want = 1,

        Loaned = 2,

        Sold = 3,

        Borrowed = 4
    }
}
