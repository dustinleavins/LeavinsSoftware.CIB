// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Models
{
    public sealed class ComicBook : Item
    {
        public ComicBook()
        {
            issues = new ObservableCollection<ComicBookIssue>();
        }

        [Required]
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

        [CustomValidation(typeof(ComicBook), "ValidateItems")]
        public ObservableCollection<ComicBookIssue> Issues
        {
            get
            {
                return issues;
            }
            set
            {
                if (issues != value)
                {
                    issues = value;
                    OnPropertyChanged("Issues");
                }
            }
        }

        public static ValidationResult ValidateItems(ObservableCollection<ComicBookIssue> items,
            ValidationContext context)
        {
            if (items.Count == 0)
            {
                return new ValidationResult("This book should have at least one issue");
            }
            else if (items.Any(i => !i.IsValid()))
            {
                return new ValidationResult("Invalid item");
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        private ItemCategory publisher;
        private ObservableCollection<ComicBookIssue> issues;
    }
}
