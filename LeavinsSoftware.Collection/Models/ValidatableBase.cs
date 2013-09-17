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
    /// Provides subclasses access to validation methods.
    /// </summary>
    public abstract class ValidatableBase : IDataErrorInfo
    {
        /// <summary>
        /// Is this instance valid?
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return ValidationResults().Count == 0;
        }

        /// <summary>
        /// Is the specified member of this instance valid?
        /// </summary>
        /// <param name="memberName">Name of member to validate</param>
        /// <returns></returns>
        public bool IsValid(string memberName)
        {
            return ValidationResultsFor(memberName).Count == 0;
        }

        /// <summary>
        /// Performs validation.
        /// </summary>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
        /// Thrown when a member of this instance is invalid.
        /// </exception>
        public void Validate()
        {
            var context = new ValidationContext(this, null, null);
            Validator.ValidateObject(this, context, true);
        }

        /// <summary>
        /// Returns a collection of validation results.
        /// </summary>
        /// <returns>
        /// An empty collection if this instance is valid; otherwise,
        /// returns a collection containing validation errors.
        /// </returns>
        public ICollection<ValidationResult> ValidationResults()
        {
            var context = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, true);

            return results;
        }

        /// <summary>
        /// Returns a collection of validation results.
        /// </summary>
        /// <param name="memberName">Name of member to validate</param>
        /// <returns>
        /// An empty collection if the specified member of this instance is valid; otherwise,
        /// returns a collection containing validation errors.
        /// </returns>
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

        /// <summary>
        /// Gets an error string for the specified member.
        /// </summary>
        /// <param name="memberName">Name of member to validate</param>
        /// <returns>
        /// An empty string if the specified member is valid;
        /// otherwise, returns an error string for the member.
        /// </returns>
        public string this[string memberName]
        {
            get
            {
                var results = this.ValidationResultsFor(memberName);

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

        /// <summary>
        /// Gets the error message for this instance.
        /// </summary>
        public string Error
        {
            get
            {
                return string.Join("\n",
                    ValidationResults().Select(r => r.ErrorMessage));
            }
        }
    }
}
