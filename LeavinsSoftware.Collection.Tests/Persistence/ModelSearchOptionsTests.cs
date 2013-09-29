// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Reflection;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using NUnit.Framework;

namespace LeavinsSoftware.Collection.Tests.Persistence
{
    [TestFixture]
    public class ModelSearchOptionsTests
    {
        [TestCase("ItemsPerPage", 0, 1, false)]
        [TestCase("ItemsPerPage", 1, 1, true)]
        [TestCase("ListType", ItemListType.Have, ItemListType.Want, false)]
        [TestCase("ListType", ItemListType.Want, ItemListType.Want, true)]
        public void EqualsTest<T>(string propertyName, T leftValue, T rightValue, bool expected)
        {
            ModelSearchOptions leftOptions = MakeOptions(propertyName, leftValue);
            ModelSearchOptions rightOptions = MakeOptions(propertyName, rightValue);
            
            Assert.AreEqual(expected, leftOptions.Equals(rightOptions));
            Assert.AreEqual(expected, leftOptions == rightOptions);
            Assert.AreEqual(!expected, leftOptions != rightOptions);
            
            Assert.AreEqual(expected, leftOptions.GetHashCode() == rightOptions.GetHashCode());
        }
        
        private static ModelSearchOptions MakeOptions<T>(string propertyName, T propertyValue)
        {
            var builder = new ModelSearchOptionsBuilder()
            {
                ItemCategory = null,
                ItemsPerPage = 20,
                ListType = ItemListType.Have,
                SearchText = string.Empty
            };
            
            // Change property
            PropertyInfo property = (typeof(ModelSearchOptionsBuilder)).GetProperty(propertyName);
            property.SetValue(builder, propertyValue, null);
            return builder.Build();
        }
    }
}
