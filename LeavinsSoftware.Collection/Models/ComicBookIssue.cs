// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LeavinsSoftware.Collection.Models
{
    public sealed class ComicBookIssue : Model
    {
        [Range(0, long.MaxValue)]
        public long ComicBookId
        {
            get
            {
                return comicBookId;
            }
            set
            {
                if (comicBookId != value)
                {
                    comicBookId = value;
                    OnPropertyChanged("ComicBookId");
                }
            }
        }

        public string IssueNumber
        {
            get
            {
                return issueNumber;
            }
            set
            {
                if (!string.Equals(issueNumber, value, StringComparison.Ordinal))
                {
                    issueNumber = value;
                    OnPropertyChanged("IssueNumber");
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

        public VolumeType IssueType
        {
            get
            {
                return issueType;
            }
            set
            {
                if (issueType != value)
                {
                    issueType = value;
                    OnPropertyChanged("IssueType");
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

        private long comicBookId;
        private string issueNumber;
        private string cover;
        private string name;
        private DistributionType distType;
        private string condition;
        private VolumeType issueType;
        private string notes;
        private ItemListType list;
    }
}
