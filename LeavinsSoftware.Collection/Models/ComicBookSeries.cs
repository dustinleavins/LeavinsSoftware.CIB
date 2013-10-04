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
    /// <summary>
    /// Represents a series of comic books
    /// </summary>
    public sealed class ComicBookSeries : Item
    {
        public ComicBookSeries()
        {
            entries = new ObservableCollection<ComicBookSeriesEntry>();
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

        [CustomValidation(typeof(ComicBookSeries), "ValidateItems")]
        public ObservableCollection<ComicBookSeriesEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        public static ValidationResult ValidateItems(ObservableCollection<ComicBookSeriesEntry> items,
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
        private ObservableCollection<ComicBookSeriesEntry> entries;
    }
}
