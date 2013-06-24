// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Models
{
    public sealed class VideoGame : Item
    {
        [Required]
        public ItemCategory System
        {
            get
            {
                return system;
            }
            set
            {
                if (system != value)
                {
                    system = value;
                    OnPropertyChanged("System");
                }
            }
        }

        public DistributionType DistributionType
        {
            get
            {
                return distributionType;
            }
            set
            {
                if (distributionType != value)
                {
                    distributionType = value;
                    OnPropertyChanged("DistributionType");
                }
            }
        }

        public string Condition
        {
            get
            {
                return condition;
            }
            set
            {
                if (!string.Equals(condition, value, StringComparison.Ordinal))
                {
                    condition = value;
                    OnPropertyChanged("Condition");
                }
            }
        }

        public string ContentProvider
        {
            get
            {
                return contentProvider;
            }
            set
            {
                if (!string.Equals(contentProvider, value, StringComparison.Ordinal))
                {
                    contentProvider = value;
                    OnPropertyChanged("ContentProvider");
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

        private ItemCategory system;
        private DistributionType distributionType;
        private string condition;
        private string contentProvider;
        private ItemListType list;
    }
}
