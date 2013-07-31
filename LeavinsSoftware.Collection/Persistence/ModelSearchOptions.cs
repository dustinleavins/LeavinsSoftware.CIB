// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public struct ModelSearchOptions : IEquatable<ModelSearchOptions>
    {
        public ModelSearchOptions(long itemsPerPage,
            ItemCategory itemCategory,
            ItemListType listType,
            bool allListTypes,
            string searchText)
        {
            ItemsPerPage = itemsPerPage;
            ItemCategory = itemCategory;
            ListType = listType;
            AllListTypes = allListTypes;
            SearchText = searchText;
        }

        public readonly long ItemsPerPage;

        public readonly ItemCategory ItemCategory;

        public readonly ItemListType ListType;

        public readonly bool AllListTypes;

        public readonly string SearchText;
        
        public override bool Equals(object obj)
        {
            return (obj is ModelSearchOptions) && Equals((ModelSearchOptions)obj);
        }
        
        public bool Equals(ModelSearchOptions other)
        {
            return this.ItemsPerPage == other.ItemsPerPage &&
                this.ItemCategory == other.ItemCategory &&
                this.ListType == other.ListType &&
                this.AllListTypes == other.AllListTypes &&
                this.SearchText == other.SearchText;
        }
        
        public override int GetHashCode()
        {
            int hashCode = 19;
            
            unchecked
            {
                hashCode += 23 * ItemsPerPage.GetHashCode();
                
                if (ItemCategory != null)
                {
                    hashCode += 23 * ItemCategory.GetHashCode();
                }
                
                hashCode += 23 * ListType.GetHashCode();
                
                hashCode += 23 * AllListTypes.GetHashCode();
                
                if (SearchText != null)
                {
                    hashCode += 23 * SearchText.GetHashCode();
                }
            }
            return hashCode;
        }
        
        public static bool operator ==(ModelSearchOptions lhs, ModelSearchOptions rhs)
        {
            return lhs.Equals(rhs);
        }
        
        public static bool operator !=(ModelSearchOptions lhs, ModelSearchOptions rhs)
        {
            return !(lhs == rhs);
        }

    }
}
