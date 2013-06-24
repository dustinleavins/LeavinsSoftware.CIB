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
    /// Variant of <see cref="ComicBook"/> to be used during searches.
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

        public long IssueCount
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
                    OnPropertyChanged("IssueCount");
                }
            }
        }

        private ItemCategory publisher;
        private long issueCount;
    }
}
