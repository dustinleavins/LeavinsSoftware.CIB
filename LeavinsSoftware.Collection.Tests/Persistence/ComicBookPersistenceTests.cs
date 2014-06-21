// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Migrations;
using LeavinsSoftware.Collection.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeavinsSoftware.Collection.Tests.Persistence
{
    [TestFixture]
    public sealed class ComicBookPersistenceTests
    {
        private IComicBookPersistence comicPersistence;
        private ItemCategory primaryPublisher;
        private ItemCategory secondaryPublisher;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            if (File.Exists(Path.Combine("default", "collection.db")))
            {
                File.Delete(Path.Combine("default", "collection.db"));
            }

            DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            Profile defaultProfile = new Profile("default");
            MigrationRunner.Run(currentDir, defaultProfile);
            comicPersistence = new ComicBookPersistence(new DirectoryInfo("."),
                new Profile("default"));

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, new Profile("default"));

            primaryPublisher = new ItemCategory()
            {
                Name = "Publisher 1",
                CategoryType = ItemCategoryType.ComicBook
            };

            categoryPersistence.Create(primaryPublisher);
            
            secondaryPublisher = new ItemCategory()
            {
                Name = "Publisher 2",
                CategoryType = ItemCategoryType.ComicBook
            };
            
            categoryPersistence.Create(secondaryPublisher);
        }

        [Test]
        public void CreateTest()
        {
            ComicBookSeries newComic = new ComicBookSeries()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryPublisher
            };

            newComic.Entries.Add(new ComicBookSeriesEntry()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                Number = "Test Issue",
                EntryType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            });

            comicPersistence.Create(newComic);
            // Create should set Ids
            Assert.IsTrue(newComic.Id > 0);
            Assert.IsTrue(newComic.Entries[0].Id > 0);
            Assert.AreEqual(newComic.Id, newComic.Entries[0].SeriesId);

            ComicBookSeries retrievedComic = comicPersistence.Retrieve(newComic.Id);
            AssertEquality.For(newComic, retrievedComic);
        }

        [Test]
        public void UpdateTest()
        {
            ComicBookSeries newComic = new ComicBookSeries()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryPublisher
            };

            newComic.Entries.Add(new ComicBookSeriesEntry()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                Number = "Test Issue",
                EntryType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            });

            comicPersistence.Create(newComic);

            // 'Set ID' checks are in CreateTest
            long comicId = newComic.Id;
            long comicIssueId = newComic.Entries[0].Id;

            ComicBookSeries updatedComic = new ComicBookSeries()
            {
                Id = comicId,
                Name = "Test Book 2",
                Notes = "Test Notes 2",
                Publisher = secondaryPublisher
            };

            updatedComic.Entries.Add(new ComicBookSeriesEntry()
            {
                Id = comicIssueId,
                SeriesId = comicId,
                Condition = "Test Condition 2",
                Cover = "Test Cover 2",
                DistributionType = DistributionType.Physical,
                Number = "Test Issue 2",
                EntryType = VolumeType.TPB,
                Name = "Title 2",
                Notes = "Test Notes 2"
            });

            comicPersistence.Update(updatedComic);

            // IDs should not change
            Assert.AreEqual(comicId, updatedComic.Id);
            Assert.AreEqual(comicIssueId, updatedComic.Entries[0].Id);
            Assert.AreEqual(comicId, updatedComic.Entries[0].SeriesId);

            ComicBookSeries retrievedComic = comicPersistence.Retrieve(comicId);
            AssertEquality.For(updatedComic, retrievedComic);
        }

        [Test]
        public void UpdateCreatesIssueTest()
        {
            ComicBookSeries comic = new ComicBookSeries()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryPublisher
            };

            comic.Entries.Add(new ComicBookSeriesEntry()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                Number = "Test Issue",
                EntryType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            });

            comicPersistence.Create(comic);
            long comicId = comic.Id;
            long initialIssueId = comic.Entries[0].Id;

            ComicBookSeriesEntry updatedIssue = new ComicBookSeriesEntry()
            {
                Id = initialIssueId,
                SeriesId = comicId,
                Condition = "Test Condition 2",
                Cover = "Test Cover 2",
                DistributionType = DistributionType.Physical,
                Number = "Test Issue 2",
                EntryType = VolumeType.TPB,
                Name = "Title 2",
                Notes = "Test Notes 2"
            };

            ComicBookSeriesEntry issueNewToUpdate = new ComicBookSeriesEntry()
            {
                Condition = "Test Condition 3",
                Cover = "Test Cover 3",
                DistributionType = DistributionType.Digital,
                Number = "Test Issue 3",
                EntryType = VolumeType.Issue,
                Name = "Title 3",
                Notes = "Test Notes 3"
            };

            comic.Entries.Clear();
            comic.Entries.Add(updatedIssue);
            comic.Entries.Add(issueNewToUpdate);
            comicPersistence.Update(comic);

            ComicBookSeries retrievedBook = comicPersistence.Retrieve(comicId);
            Assert.IsNotNull(retrievedBook);
            Assert.AreEqual(comicId, retrievedBook.Id);
            Assert.IsNotNull(retrievedBook.Entries);
            Assert.AreEqual(2, retrievedBook.Entries.Count);

            ComicBookSeriesEntry initialRetrievedIssue = retrievedBook.Entries
                .Single(i => i.Id == updatedIssue.Id);

            Assert.AreEqual(retrievedBook.Id, initialRetrievedIssue.SeriesId);
            AssertEquality.For(updatedIssue, initialRetrievedIssue);

            ComicBookSeriesEntry secondRetrievedIssue = retrievedBook.Entries
                .Single(i => i.Id == issueNewToUpdate.Id);

            Assert.AreEqual(retrievedBook.Id, secondRetrievedIssue.SeriesId);
            AssertEquality.For(issueNewToUpdate, secondRetrievedIssue);
        }

        [Test]
        public void UpdateDeletesIssueTest()
        {
            ComicBookSeries comic = new ComicBookSeries()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryPublisher
            };

            ComicBookSeriesEntry firstIssue = new ComicBookSeriesEntry()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                Number = "Test Issue",
                EntryType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            };

            ComicBookSeriesEntry issueToDelete = new ComicBookSeriesEntry()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                Number = "Test Issue",
                EntryType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            };

            comic.Entries.Add(firstIssue);
            comic.Entries.Add(issueToDelete);

            comicPersistence.Create(comic);

            comic.Entries.Remove(firstIssue);

            comicPersistence.Update(comic);

            ComicBookSeries retrievedBook = comicPersistence.Retrieve(comic.Id);
            Assert.IsNotNull(retrievedBook);
            Assert.IsNotNull(retrievedBook.Entries);
            Assert.AreEqual(1, retrievedBook.Entries.Count);
            Assert.IsFalse(retrievedBook.Entries.Any(i => i.Id == firstIssue.Id));
            Assert.IsTrue(retrievedBook.Entries.Any(i => i.Id == issueToDelete.Id));
        }

        [Test]
        public void DeleteTest()
        {
            ComicBookSeries newComic = new ComicBookSeries()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryPublisher
            };

            newComic.Entries.Add(new ComicBookSeriesEntry());

            comicPersistence.Create(newComic);
            comicPersistence.Delete(newComic);

            Assert.IsNull(comicPersistence.Retrieve(newComic.Id));
        }

        [Test]
        public void RetrieveNullTest()
        {
            Assert.IsNull(comicPersistence.Retrieve(long.MaxValue));
        }
        
        [TestCase(long.MinValue)]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [ExpectedException(typeof(ArgumentException))]
        public void RetrieveInvalidTest(long invalidId)
        {
            Assert.IsNull(comicPersistence.Retrieve(invalidId));
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNullTest()
        {
            comicPersistence.Create(null);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullTest()
        {
            comicPersistence.Update(null);
        }
        
        [Test]
        public void PaginationTest()
        {
            #region Test Setup
            if (File.Exists(Path.Combine("pagination", "collection.db")))
            {
                File.Delete(Path.Combine("pagination", "collection.db"));
            }

            DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            Profile paginationProfile = new Profile("pagination");
            MigrationRunner.Run(currentDir, paginationProfile);

            IComicBookPersistence target =
                new ComicBookPersistence(currentDir, paginationProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, paginationProfile);

            List<ItemCategory> categories = new List<ItemCategory>()
            {
                new ItemCategory()
                {
                    Name = "Category 1",
                    CategoryType = ItemCategoryType.ComicBook,
                    Code = "001"
                },

                new ItemCategory()
                {
                    Name = "Category 2",
                    CategoryType = ItemCategoryType.ComicBook,
                    Code = "002"
                }
            };

            foreach (ItemCategory category in categories)
            {
                categoryPersistence.Create(category);
            }

            List<ComicBookSeries> books = new List<ComicBookSeries>();
            books.Add(new ComicBookSeries()
            {
                Name = "Test Book 1",
                Notes = "Test Notes",
                Publisher = categories[0]
            });

            books.Add(new ComicBookSeries()
            {
                Name = "Test Book 2",
                Notes = "Test Notes",
                Publisher = categories[0]
            });

            books.Add(new ComicBookSeries()
            {
                Name = "Test Book 3",
                Notes = "Test Notes",
                Publisher = categories[0]
            });

            books.Add(new ComicBookSeries()
            {
                Name = "Test Book 4",
                Notes = "Test Notes",
                Publisher = categories[1]
            });

            foreach (ComicBookSeries book in books)
            {
                // Add issues to each book before adding the book
                book.Entries.Add(new ComicBookSeriesEntry()
                {
                    Number = "1",
                    ListType = ItemListType.Have
                });

                book.Entries.Add(new ComicBookSeriesEntry()
                {
                    Number = "2",
                    ListType = ItemListType.Have
                });

                book.Entries.Add(new ComicBookSeriesEntry()
                {
                    Number = "3",
                    ListType = ItemListType.Want
                });

                target.Create(book);
            }

            #endregion

            List<ComicBookSeries> tempResults = new List<ComicBookSeries>();

            // Null Category
            ModelSearchOptions allInclusiveOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1
            }.Build();

            Assert.AreEqual(4, target.TotalResults(allInclusiveOptions));

            for (long pageNumber = 0; pageNumber < 4; ++pageNumber)
            {
                List<ComicBookSeries> currentPage = target.Page(allInclusiveOptions, pageNumber);
                Assert.AreEqual(1, currentPage.Count);
                tempResults.AddRange(currentPage);
            }

            Assert.AreEqual(4, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == books[0].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == books[1].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == books[2].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == books[3].Id));

            // Non-Null Category
            tempResults.Clear();

            ModelSearchOptions categoryOneOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                ItemCategory = categories[0]
            }.Build();

            Assert.AreEqual(3, target.TotalResults(categoryOneOptions));

            for (long pageNumber = 0; pageNumber < 3; ++pageNumber)
            {
                List<ComicBookSeries> currentPage = target.Page(categoryOneOptions, pageNumber);
                Assert.AreEqual(1, currentPage.Count);
                tempResults.AddRange(currentPage);
            }

            Assert.AreEqual(3, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == books[0].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == books[1].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == books[2].Id));
            Assert.IsFalse(tempResults.Any(i => i.Id == books[3].Id)); // has 'category 2'

            // Proper pagination checks
            ModelSearchOptions multiItemsOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 2
            }.Build();

            Assert.AreEqual(4, target.TotalResults(multiItemsOptions));
            Assert.AreEqual(2, target.Page(multiItemsOptions, 0).Count);
            Assert.AreEqual(2, target.Page(multiItemsOptions, 1).Count);

            // Name Search
            ModelSearchOptions nameSearch = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                SearchText = books[0].Name
            }.Build();

            tempResults = target.Page(nameSearch, 0);
            Assert.AreEqual(1, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == books[0].Id));
        }

        [Test]
        public void ItemListTypePaginationTest()
        {
            #region Test Setup

            if (File.Exists(Path.Combine("listtype", "collection.db")))
            {
                File.Delete(Path.Combine("listtype", "collection.db"));
            }

            DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            Profile listProfile = new Profile("listtype");
            MigrationRunner.Run(currentDir, listProfile);

            IComicBookPersistence target =
                new ComicBookPersistence(currentDir, listProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, listProfile);

            ItemCategory category = categoryPersistence.Create(new ItemCategory()
            {
                Name = "Category 1",
                CategoryType = ItemCategoryType.ComicBook,
                Code = "001"
            });

            ComicBookSeries addedBook = new ComicBookSeries()
            {
                Name = "Test Book",
                Publisher = category
            };

            foreach (ItemListType listType in Enum.GetValues(typeof(ItemListType)).Cast<ItemListType>())
            {
                addedBook.Entries.Add(new ComicBookSeriesEntry()
                {
                    Number = listType.ToString(),
                    ListType = listType
                });
            }

            // Add an additional 'Have' issue
            addedBook.Entries.Add(new ComicBookSeriesEntry()
            {
                Number = "Have - 2nd",
                ListType = ItemListType.Have
            });

            target.Create(addedBook);

            #endregion

            foreach (ItemListType listType in Enum.GetValues(typeof(ItemListType)).Cast<ItemListType>())
            {
                ModelSearchOptions searchOptions = new ModelSearchOptionsBuilder()
                {
                    ListType = listType,
                    ItemsPerPage = 20
                }.Build();

                string errorMsg = string.Format("Type: {0}", listType);

                Assert.AreEqual(1, target.TotalResults(searchOptions), errorMsg);
                Assert.AreEqual(1, target.Page(searchOptions, 0).Count, errorMsg);

                ComicBookSeries retrievedSummary = target.Page(searchOptions, 0).First();

                if (listType == ItemListType.Have)
                {
                    Assert.AreEqual(2, retrievedSummary.EntriesCount, errorMsg);
                }
                else
                {
                    Assert.AreEqual(1, retrievedSummary.EntriesCount, errorMsg);
                }
            }
            
            // All-inclusive search
            ModelSearchOptions allInclusiveOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20
            }.Build();
            
            Assert.AreEqual(1, target.TotalResults(allInclusiveOptions));
            Assert.AreEqual(1, target.Page(allInclusiveOptions, 0).Count);
            
            ComicBookSeries issueSummary = target.Page(allInclusiveOptions, 0).First();
            Assert.AreEqual(6, issueSummary.EntriesCount);
            
            // Name search search
            ModelSearchOptions nameOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20,
                SearchText = "Test"
            }.Build();
            
            Assert.AreEqual(1, target.TotalResults(nameOptions));
            Assert.AreEqual(1, target.Page(nameOptions, 0).Count);
            
            Assert.AreEqual(6, target.Page(nameOptions, 0).First().EntriesCount);
            
            // Name not found search
            ModelSearchOptions nameNotFoundOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20,
                SearchText = "Name not found"
            }.Build();
            
            Assert.AreEqual(0, target.TotalResults(nameNotFoundOptions));
            Assert.AreEqual(0, target.Page(nameNotFoundOptions, 0).Count);
        }
    }
}
