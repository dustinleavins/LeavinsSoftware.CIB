// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using LeavinsSoftware.Collection.Resources;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Represents an item in a user's collection.
    /// </summary>
    public abstract class Item :  Model
    {
        private static readonly char[] summarySeparator =
            new char[] { '\n' };
        
        public static ItemCategoryType CategoryType<T>() where T: Item
        {
            var attr = (ItemCategoryTypeAttribute)typeof(T).GetCustomAttributes(true)
                .Single(x => x is ItemCategoryTypeAttribute);
            
            return attr.CategoryType;
        }
        
        [Required]
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
                    OnPropertyChanged("NotesSummary");
                }
            }
        }

        /// <summary>
        /// Single-line summary for notes
        /// </summary>
        public string NotesSummary
        {
            get
            {
                if (string.IsNullOrEmpty(notes) || !notes.Contains("\n"))
                {
                    return notes;
                }
                else
                {
                    string firstLine = notes
                        .Split(summarySeparator, StringSplitOptions.RemoveEmptyEntries)[0]
                        .TrimEnd();

                    return string.Format(ModelResources.Item_SummaryFormat, firstLine);
                }
            }
        }

        private string name;
        private string notes;
    }
}
