// Copyright (c) 2013, 2014 Dustin Leavins
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
    [ItemCategoryType(ItemCategoryType.ComicBook)]
    [CustomValidation(typeof(ComicBookSeries), "ValidateItems")]
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

        public ObservableCollection<ComicBookSeriesEntry> Entries
        {
            get
            {
                return entries;
            }
        }
        
        public static ComicBookSeries NewSummary()
        {
            ComicBookSeries series = new ComicBookSeries();
            series.isSummary = true;
            return series;
        }

        public static ValidationResult ValidateItems(ComicBookSeries series,
            ValidationContext context)
        {
            if (series.IsSummary)
            {
                return ValidationResult.Success;
            }
            
            var items = series.Entries;
            if (items.Count == 0)
            {
                return new ValidationResult("This book should have at least one issue",
                    new string[] { "Entries" });
            }
            else if (items.Any(i => !i.IsValid()))
            {
                return new ValidationResult("Invalid item",
                    new string[] { "Entries" });
            }
            else
            {
                return ValidationResult.Success;
            }
        }
        
        public override bool IsSummary
        {
            get
            {
                return isSummary;
            }
        }

        private ItemCategory publisher;
        private ObservableCollection<ComicBookSeriesEntry> entries;
        private bool isSummary;
    }
}
