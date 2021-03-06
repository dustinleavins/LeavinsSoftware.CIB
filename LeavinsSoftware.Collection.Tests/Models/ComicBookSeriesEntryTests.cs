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
    public sealed class ComicBookSeriesEntryTests
    {
        private ComicBookSeriesEntry target;

        [SetUp]
        public void SetUp()
        {
            target = new ComicBookSeriesEntry()
            {
                Number = "1",
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
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public void NameTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Name");

            target.Name = expectedValue;
            Assert.AreEqual(expectedValue, target.Name);

            Assert.IsTrue(target.IsValid());

            Assert.AreEqual(expectedValue != null,
                PropertyChangedChecker.HasPropertyChanged("Name"));
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
        public void SeriesIdTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("SeriesId");
            target.SeriesId = expectedValue;
            Assert.AreEqual(expectedValue, target.SeriesId);
            Assert.IsTrue(target.IsValid());

            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("SeriesId"));
        }

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(int.MinValue)]
        [TestCase(long.MinValue)]
        public void SeriesIdInvalidTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("SeriesId");

            target.SeriesId = expectedValue;
            Assert.AreEqual(expectedValue, target.SeriesId);

            var results = target.ValidationResults();
            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("SeriesId"));
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
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Issue")]
        public void NumberTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("Number");

            target.Number = expectedValue;
            Assert.AreEqual(expectedValue, target.Number);

            Assert.IsTrue(target.IsValid());

            // IssueNumber is set in SetUp
            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Number"));
        }
        
        [TestCase(VolumeType.Issue)]
        [TestCase(VolumeType.TPB)]
        public void IssueTypeTest(VolumeType expected)
        {
            PropertyChangedChecker.CheckOnly("EntryType");
            target.EntryType = expected;
            Assert.AreEqual(expected, target.EntryType);
            Assert.IsTrue(target.IsValid());
            Assert.AreEqual(expected != default(VolumeType),
                PropertyChangedChecker.HasPropertyChanged("EntryType"));
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Variant A")]
        public void CoverTest(string expected)
        {
            PropertyChangedChecker.CheckOnly("Cover");
            target.Cover = expected;
            Assert.AreEqual(expected, target.Cover);
            Assert.IsTrue(target.IsValid());
            
            Assert.AreEqual(expected != null,
                PropertyChangedChecker.HasPropertyChanged("Cover"));
        }
    }
}
