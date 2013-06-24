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
    public abstract class ModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid()
        {
            return ValidationResults().Count == 0;
        }

        public bool IsValid(string memberName)
        {
            return ValidationResultsFor(memberName).Count == 0;
        }

        public void Validate()
        {
            var context = new ValidationContext(this, null, null);
            Validator.ValidateObject(this, context, true);
        }

        public ICollection<ValidationResult> ValidationResults()
        {
            var context = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, true);

            return results;
        }

        public ICollection<ValidationResult> ValidationResultsFor(string memberName)
        {
            PropertyInfo property = GetType().GetProperty(memberName);

            if (property == null)
            {
                throw new ArgumentException("memberName must match a property for this type.",
                                            "memberName");
            }
            else if (property.GetIndexParameters().Length != 0)
            {
                throw new ArgumentException("This method does not currently support indexed properties.",
                                            "memberName");
            }

            ValidationContext context = new ValidationContext(this, null, null)
            {
                MemberName = memberName
            };

            ICollection<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateProperty(property.GetValue(this, null),
                context,
                results);

            return results;
        }

        public string this[string columnName]
        {
            get
            {
                var results = this.ValidationResultsFor(columnName);

                if (results.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return results.First().ErrorMessage;
                }
            }
        }

        public string Error
        {
            get
            {
                return string.Join("\n",
                    ValidationResults().Select(r => r.ErrorMessage));
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
    }
}
