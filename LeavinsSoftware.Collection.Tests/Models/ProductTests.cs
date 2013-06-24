// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Tests.Models
{
    [TestFixture]
    public sealed class ProductTests
    {
        private Product target;

        [SetUp]
        public void SetUp()
        {
            target = new Product()
            {
                Name = "Test Item",

                Category = new ItemCategory()
                {
                    Name = "Category",
                    CategoryType = ItemCategoryType.Product
                },

                ListType = ItemListType.Have
            };
            
            PropertyChangedChecker.ListenTo(target);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(int.MaxValue)]
        [TestCase(long.MaxValue)]
        public void IdTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Id");
            PropertyChangedChecker.CheckOnly("HasId");
            target.Id = expectedValue;
            Assert.AreEqual(expectedValue, target.Id);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("Id"));
            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("HasId"));
        }
        
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(int.MinValue)]
        [TestCase(long.MinValue)]
        public void IdInvalidTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Id");
            PropertyChangedChecker.CheckOnly("HasId");

            target.Id = expectedValue;
            Assert.AreEqual(expectedValue, target.Id);

            var results = target.ValidationResults();
            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Id"));
            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("HasId"));
        }
        
        [TestCase("Name")]
        public void NameTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Name");

            target.Name = expectedValue;
            Assert.AreEqual(expectedValue, target.Name);

            Assert.IsTrue(target.IsValid());

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Name"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void NameRequiredTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Name");
            target.Name = expectedValue;
            Assert.AreEqual(expectedValue, target.Name);

            var results = target.ValidationResults();
            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Name"));
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Notes")]
        public void NotesTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Notes");

            target.Notes = expectedValue;
            Assert.AreEqual(expectedValue, target.Notes);

            Assert.IsTrue(target.IsValid());

            Assert.AreEqual(expectedValue != null,
                PropertyChangedChecker.HasPropertyChanged("Notes"));
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(int.MaxValue)]
        [TestCase(long.MaxValue)]
        public void QuantityTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Quantity");
            target.Quantity = expectedValue;
            Assert.AreEqual(expectedValue, target.Quantity);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("Quantity"));
        }
        
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(int.MinValue)]
        [TestCase(long.MinValue)]
        public void QuantityInvalidTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Quantity");

            target.Quantity = expectedValue;
            Assert.AreEqual(expectedValue, target.Quantity);

            var results = target.ValidationResults();
            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Quantity"));
        }
        
        [Test]
        public void CategoryTest()
        {
            ItemCategory newCategory = new ItemCategory()
            {
                CategoryType = ItemCategoryType.Product,
                Name = "Second Category"
            };

            PropertyChangedChecker.CheckOnly("Category");
            target.Category = newCategory;

            Assert.AreEqual(newCategory, target.Category);
            Assert.IsTrue(target.IsValid());

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Category"));
        }
        
        [TestCase(ItemListType.Want)]
        [TestCase(ItemListType.Have)]
        public void ListTypeTest(ItemListType expected)
        {
            PropertyChangedChecker.CheckOnly("ListType");
            target.ListType = expected;
            Assert.AreEqual(expected, target.ListType);
            Assert.IsTrue(target.IsValid());
            Assert.AreEqual(expected != default(ItemListType),
                PropertyChangedChecker.HasPropertyChanged("ListType"));
        }
    }
}
