// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Represents a miscellaneous physical product that is part of a user's
    /// collection.
    /// </summary>
    /// <remarks>
    /// This usually represents a collectible of some kind.
    /// </remarks>
    public sealed class Product : Item
    {
        [Required]
        public ItemCategory Category
        {
            get
            {
                return category;
            }
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        [Range(0, long.MaxValue)]
        public long Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }

        public ItemListType ListType
        {
            get
            {
                return list;
            }
            set
            {
                if (list != value)
                {
                    list = value;
                    OnPropertyChanged("ListType");
                }
            }
        }

        private ItemCategory category;
        private long quantity;
        private ItemListType list;
    }
}
