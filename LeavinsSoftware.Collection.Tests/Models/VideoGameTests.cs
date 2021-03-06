﻿// Copyright (c) 2013, 2014 Dustin Leavins
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
    public sealed class VideoGameTests
    {
        private VideoGame target;

        [SetUp]
        public void SetUp()
        {
            target = new VideoGame()
            {
                Name = "Test Item",

                System = new ItemCategory()
                {
                    Name = "Category",
                    CategoryType = ItemCategoryType.VideoGame
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
            PropertyChangedChecker.CheckOnly("IsNew");
            target.Id = expectedValue;
            Assert.AreEqual(expectedValue, target.Id);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("Id"));
            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("IsNew"));
        }
        
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(int.MinValue)]
        [TestCase(long.MinValue)]
        public void IdInvalidTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Id");
            PropertyChangedChecker.CheckOnly("IsNew");

            target.Id = expectedValue;
            Assert.AreEqual(expectedValue, target.Id);

            var results = target.ValidationResults();
            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Id"));
            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("IsNew"));
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
            PropertyChangedChecker.CheckOnly("NotesSummary");

            target.Notes = expectedValue;
            Assert.AreEqual(expectedValue, target.Notes);

            Assert.IsTrue(target.IsValid());

            Assert.AreEqual(expectedValue != null,
                PropertyChangedChecker.HasPropertyChanged("Notes"));
        }
        
        [Test]
        public void SystemTest()
        {
            ItemCategory newCategory = new ItemCategory()
            {
                CategoryType = ItemCategoryType.VideoGame,
                Name = "Second Category"
            };

            PropertyChangedChecker.CheckOnly("System");
            target.System = newCategory;

            Assert.AreEqual(newCategory, target.System);
            Assert.IsTrue(target.IsValid());

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("System"));
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
        
        [TestCase(DistributionType.Digital)]
        [TestCase(DistributionType.Physical)]
        public void DistributionTypeTest(DistributionType expected)
        {
            PropertyChangedChecker.CheckOnly("DistributionType");
            target.DistributionType = expected;
            Assert.AreEqual(expected, target.DistributionType);
            Assert.IsTrue(target.IsValid());
            Assert.AreEqual(expected != default(DistributionType),
                PropertyChangedChecker.HasPropertyChanged("DistributionType"));
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Good")]
        public void ConditionTest(string expected)
        {
            PropertyChangedChecker.CheckOnly("Condition");
            target.Condition = expected;
            Assert.AreEqual(expected, target.Condition);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expected != null,
                PropertyChangedChecker.HasPropertyChanged("Condition"));
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("XLock Live")]
        public void ContentProviderTest(string expected)
        {
            PropertyChangedChecker.CheckOnly("Condition");
            target.Condition = expected;
            Assert.AreEqual(expected, target.Condition);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expected != null,
                PropertyChangedChecker.HasPropertyChanged("Condition"));
        }
    }
}
