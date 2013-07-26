// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LeavinsSoftware.Collection.Models
{
    public sealed class ItemCategory : Model, IEquatable<ItemCategory>
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

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                if (!string.Equals(code, value, StringComparison.Ordinal))
                {
                    code = value;
                    OnPropertyChanged("Code");
                }
            }
        }

        public ItemCategoryType CategoryType
        {
            get
            {
                return categoryType;
            }
            set
            {
                if (categoryType != value)
                {
                    categoryType = value;
                    OnPropertyChanged("CategoryType");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemCategory);
        }

        public bool Equals(ItemCategory other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id &&
                string.Equals(name, other.name, StringComparison.Ordinal) &&
                string.Equals(code, other.code, StringComparison.Ordinal) &&
                categoryType == other.categoryType;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ItemCategory lhs, ItemCategory rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            else if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                return false;
            }
            else
            {
                return lhs.Equals(rhs);
            }
        }

        public static bool operator !=(ItemCategory lhs, ItemCategory rhs)
        {
            return !(lhs == rhs);
        }

        private string name;
        private string code;
        private ItemCategoryType categoryType;
    }
}
