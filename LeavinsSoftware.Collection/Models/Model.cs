// Copyright (c) 2013, 2014, 2021 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Represents a data model.
    /// </summary>
    public abstract class Model : ValidatableBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Range(typeof(long), "0", "9223372036854775807")]
        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                bool wasNewModel = IsNew;

                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");

                    if (wasNewModel)
                    {
                        OnPropertyChanged("IsNew");
                    }
                }
            }
        }
        
        public bool IsNew
        {
        	get
        	{
        		return id == 0;
        	}
        }
        
        public virtual bool IsSummary { get { return false; } }

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
