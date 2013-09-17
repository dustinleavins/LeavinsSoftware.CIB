// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Represents a data model.
    /// </summary>
    public abstract class Model : ValidatableBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Range(0, long.MaxValue)]
        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                bool isNewModel = !HasId;

                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");

                    if (isNewModel)
                    {
                        OnPropertyChanged("HasId");
                    }
                }
            }
        }

        public bool HasId
        {
            get
            {
                return id != 0;
            }
        }

        /// <summary>
        /// Triggers PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private long id;
    }
}
