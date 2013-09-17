// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.ComponentModel.DataAnnotations;
using System;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Represents an item in a user's collection.
    /// </summary>
    public abstract class Item :  Model
    {
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
                }
            }
        }

        private string name;
        private string notes;
    }
}
