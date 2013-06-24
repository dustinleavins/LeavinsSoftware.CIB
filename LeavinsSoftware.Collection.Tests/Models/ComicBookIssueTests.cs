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
    public sealed class ComicBookIssueTests
    {
        private ComicBookIssue target;

        [SetUp]
        public void SetUp()
        {
            target = new ComicBookIssue()
            {
                IssueNumber = "1",
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
        public void ComicBookIdIdTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("ComicBookId");
            target.ComicBookId = expectedValue;
            Assert.AreEqual(expectedValue, target.ComicBookId);
            Assert.IsTrue(target.IsValid());

            Assert.AreEqual(expectedValue != default(long),
                PropertyChangedChecker.HasPropertyChanged("ComicBookId"));
        }

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(int.MinValue)]
        [TestCase(long.MinValue)]
        public void ComicBookIdIdInvalidTest(long expectedValue)
        {
            PropertyChangedChecker.CheckOnly("ComicBookId");

            target.ComicBookId = expectedValue;
            Assert.AreEqual(expectedValue, target.ComicBookId);

            var results = target.ValidationResults();
            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("ComicBookId"));
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
        public void IssueNumberTest(string expectedValue)
        {
            PropertyChangedChecker.CheckOnly("IssueNumber");

            target.IssueNumber = expectedValue;
            Assert.AreEqual(expectedValue, target.IssueNumber);

            Assert.IsTrue(target.IsValid());

            // IssueNumber is set in SetUp
            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("IssueNumber"));
        }
        
        [TestCase(VolumeType.Issue)]
        [TestCase(VolumeType.TPB)]
        public void IssueTypeTest(VolumeType expected)
        {
            PropertyChangedChecker.CheckOnly("IssueType");
            target.IssueType = expected;
            Assert.AreEqual(expected, target.IssueType);
            Assert.IsTrue(target.IsValid());
            Assert.AreEqual(expected != default(VolumeType),
                PropertyChangedChecker.HasPropertyChanged("IssueType"));
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
