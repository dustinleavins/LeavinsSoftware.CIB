// Copyright (c) 2013, 2014 Dustin Leavins
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
    public sealed class ComicBookSeriesTests
    {
        private ComicBookSeries target;

        [SetUp]
        public void SetUp()
        {
            target = new ComicBookSeries()
            {
                Name = "Test Item",

                Publisher = new ItemCategory()
                {
                    Name = "Category",
                    CategoryType = ItemCategoryType.ComicBook
                }
            };
            
            target.Entries.Add(new ComicBookSeriesEntry()
            {
                Number = "1",
                ListType = ItemListType.Have
            });
            
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
        public void PublisherTest()
        {
            ItemCategory newPublisher = new ItemCategory()
            {
                CategoryType = ItemCategoryType.ComicBook,
                Name = "Second Category"
            };

            PropertyChangedChecker.CheckOnly("Publisher");
            target.Publisher = newPublisher;

            Assert.AreEqual(newPublisher, target.Publisher);
            Assert.IsTrue(target.IsValid());

            Assert.IsTrue(PropertyChangedChecker.HasPropertyChanged("Publisher"));
        }
        
        [Test]
        public void IssuesEmptyTest()
        {
            ComicBookSeries comicBookWithoutIssues = new ComicBookSeries()
            {
                Name = "Test Item",

                Publisher = new ItemCategory()
                {
                    Name = "Category",
                    CategoryType = ItemCategoryType.ComicBook
                }
            };
            
            Assert.IsFalse(comicBookWithoutIssues.IsValid());
        }
        
        [Test]
        public void IssuesInvalidTest()
        {
            ComicBookSeries comicBookWithInvalidIssue = new ComicBookSeries()
            {
                Name = "Test Item",

                Publisher = new ItemCategory()
                {
                    Name = "Category",
                    CategoryType = ItemCategoryType.ComicBook
                }
            };
            comicBookWithInvalidIssue.Entries.Add(new ComicBookSeriesEntry() { Id = -1 });
            
            Assert.IsFalse(comicBookWithInvalidIssue.IsValid());
        }
        
        [Test]
        public void SummaryValidationTest()
        {
            ComicBookSeries summary = ComicBookSeries.NewSummary();
            summary.Name = "Test Comic Book";
            summary.Publisher = new ItemCategory()
            {
                Name = "Category",
                CategoryType = ItemCategoryType.ComicBook
            };
            
            Assert.IsTrue(target.IsValid());
        }
    }
}
