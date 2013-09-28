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
    public sealed class ItemCategoryTests
    {
        private ItemCategory target;

        [SetUp]
        public void SetUp()
        {
            target = new ItemCategory()
            {
                Name = "Test Name"
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
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Code")]
        public void CodeTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Code");

            target.Code = expectedValue;
            Assert.AreEqual(expectedValue, target.Code);

            Assert.IsTrue(target.IsValid());

            Assert.AreEqual(expectedValue != null,
                PropertyChangedChecker.HasPropertyChanged("Code"));
        }
        
        [TestCase(ItemCategoryType.ComicBook)]
        [TestCase(ItemCategoryType.Product)]
        [TestCase(ItemCategoryType.VideoGame)]
        public void CategoryTypeTest(ItemCategoryType expectedValue)
        {
            PropertyChangedChecker.CheckOnly("CategoryType");
            
            target.CategoryType = expectedValue;
            Assert.AreEqual(expectedValue, target.CategoryType);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expectedValue != default(ItemCategoryType),
                PropertyChangedChecker.HasPropertyChanged("CategoryType"));
        }
        
        [TestCase("Id", 0, 1, false)]
        [TestCase("Id", 1, 1, true)]
        [TestCase("Code", "", "", true)]
        [TestCase("Code", "A", "B", false)]
        [TestCase("CategoryType", ItemCategoryType.ComicBook, ItemCategoryType.ComicBook, true)]
        [TestCase("CategoryType", ItemCategoryType.ComicBook, ItemCategoryType.VideoGame, false)]
        public void EqualsTest<T>(string propertyName, T leftValue, T rightValue, bool expected)
        {
            ItemCategory objectB =  new ItemCategory()
            {
                Name = "Test Name"
            };
            
            EqualsTester.New(target, objectB)
                .Property(propertyName)
                .LeftHandValue(leftValue)
                .RightHandValue(rightValue)
                .Expect(expected)
                .Do();
        }
        
        [Test]
        public void EqualsNullTest()
        {
            Assert.IsFalse(target.Equals((ItemCategory)null));
        }
    }
}
