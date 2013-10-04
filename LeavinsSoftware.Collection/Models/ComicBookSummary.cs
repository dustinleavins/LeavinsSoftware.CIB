// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Variant of <see cref="ComicBookSeries"/> to be used during searches.
    /// </summary>
    public sealed class ComicBookSummary : Item
    {
        public ItemCategory Publisher
        {
            get
            {
                return publisher;
            }
            set
            {
                if (publisher != value)
                {
                    publisher = value;
                    OnPropertyChanged("Publisher");
                }
            }
        }

        public long EntriesCount
        {
            get
            {
                return issueCount;
            }
            set
            {
                if (issueCount != value)
                {
                    issueCount = value;
                    OnPropertyChanged("EntriesCount");
                }
            }
        }

        private ItemCategory publisher;
        private long issueCount;
    }
}
