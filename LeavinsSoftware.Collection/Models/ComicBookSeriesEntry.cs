// Copyright (c) 2013, 2014, 2021 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.ComponentModel.DataAnnotations;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Represents a single entry (issue or volume) in a series of
    /// comic books.
    /// </summary>
    public sealed class ComicBookSeriesEntry : Model
    {
        [Range(typeof(long), "0", "9223372036854775807")]
        public long SeriesId
        {
            get
            {
                return seriesId;
            }
            set
            {
                if (seriesId != value)
                {
                    seriesId = value;
                    OnPropertyChanged("SeriesId");
                }
            }
        }

        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                if (!string.Equals(number, value, StringComparison.Ordinal))
                {
                    number = value;
                    OnPropertyChanged("Number");
                }
            }
        }

        public string Cover
        {
            get
            {
                return cover;
            }
            set
            {
                if (!string.Equals(cover, value, StringComparison.Ordinal))
                {
                    cover = value;
                    OnPropertyChanged("Cover");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!string.Equals(name, value, StringComparison.Ordinal))
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public DistributionType DistributionType
        {
            get
            {
                return distType;
            }
            set
            {
                if (distType != value)
                {
                    distType = value;
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

        public VolumeType EntryType
        {
            get
            {
                return entryType;
            }
            set
            {
                if (entryType != value)
                {
                    entryType = value;
                    OnPropertyChanged("EntryType");
                }
            }
        }

        public string Notes
        {
            get
            {
                return notes;
            }
            set
            {
                if (!string.Equals(notes, value, StringComparison.Ordinal))
                {
                    notes = value;
                    OnPropertyChanged("Notes");
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

        private long seriesId;
        private string number;
        private string cover;
        private string name;
        private DistributionType distType;
        private string condition;
        private VolumeType entryType;
        private string notes;
        private ItemListType list;
    }
}
