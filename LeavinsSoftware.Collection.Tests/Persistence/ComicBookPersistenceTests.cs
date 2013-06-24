// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Migrations;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeavinsSoftware.Collection.Tests.Persistence
{
    [TestFixture]
    public sealed class ComicBookPersistenceTests
    {
        private IComicBookPersistence comicPersistence;
        private ItemCategory primaryCategory;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            if (File.Exists(Path.Combine("default", "collection.db")))
            {
                File.Delete(Path.Combine("default", "collection.db"));
            }

            MigrationRunner.Run(".", "default");
            comicPersistence = new ComicBookPersistence(".", "default");

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(".", "default");

            primaryCategory = new ItemCategory()
            {
                Name = "Goods",
            };

            categoryPersistence.Create(primaryCategory);
        }

        [Test]
        public void CreateTest()
        {
            ComicBook newComic = new ComicBook()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryCategory
            };

            newComic.Issues.Add(new ComicBookIssue()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                IssueNumber = "Test Issue",
                IssueType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            });

            comicPersistence.Create(newComic);
            // Create should set Ids
            Assert.IsTrue(newComic.Id > 0);
            Assert.IsTrue(newComic.Issues[0].Id > 0);
            Assert.AreEqual(newComic.Id, newComic.Issues[0].ComicBookId);

            ComicBook retrievedComic = comicPersistence.Retrieve(newComic.Id);
            Assert.IsNotNull(retrievedComic);
            Assert.AreEqual(newComic.Id, retrievedComic.Id);
            Assert.AreEqual(newComic.Name, retrievedComic.Name);
            Assert.AreEqual(newComic.Notes, retrievedComic.Notes);

            Assert.IsNotNull(retrievedComic.Publisher);
            Assert.AreEqual(newComic.Publisher.Name, retrievedComic.Publisher.Name);
            // newComic.Publisher.Code is null; retrievedComic.Publisher.Code is empty
            Assert.AreEqual(newComic.Publisher.Id, retrievedComic.Publisher.Id);

            Assert.IsNotNull(retrievedComic.Issues);
            Assert.AreEqual(1, retrievedComic.Issues.Count);

            ComicBookIssue retrievedIssue = retrievedComic.Issues[0];
            Assert.AreEqual(retrievedComic.Id, retrievedIssue.ComicBookId);
            AssertEquality(newComic.Issues[0], retrievedIssue);
        }

        [Test]
        public void UpdateTest()
        {
            ComicBook newComic = new ComicBook()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryCategory
            };

            newComic.Issues.Add(new ComicBookIssue()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                IssueNumber = "Test Issue",
                IssueType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            });

            comicPersistence.Create(newComic);

            // 'Set ID' checks are in CreateTest
            long comicId = newComic.Id;
            long comicIssueId = newComic.Issues[0].Id;

            ComicBook updatedComic = new ComicBook()
            {
                Id = comicId,
                Name = "Test Book 2",
                Notes = "Test Notes 2",
                // TODO: Check publisher change
                Publisher = primaryCategory
            };

            updatedComic.Issues.Add(new ComicBookIssue()
            {
                Id = comicIssueId,
                ComicBookId = comicId,
                Condition = "Test Condition 2",
                Cover = "Test Cover 2",
                DistributionType = DistributionType.Physical,
                IssueNumber = "Test Issue 2",
                IssueType = VolumeType.TPB,
                Name = "Title 2",
                Notes = "Test Notes 2"
            });

            comicPersistence.Update(updatedComic);

            // IDs should not change
            Assert.AreEqual(comicId, updatedComic.Id);
            Assert.AreEqual(comicIssueId, updatedComic.Issues[0].Id);
            Assert.AreEqual(comicId, updatedComic.Issues[0].ComicBookId);

            ComicBook retrievedComic = comicPersistence.Retrieve(comicId);
            Assert.IsNotNull(retrievedComic);
            
            Assert.AreEqual(comicId, retrievedComic.Id);
            Assert.AreEqual(updatedComic.Id, retrievedComic.Id);
            Assert.AreEqual(updatedComic.Name, retrievedComic.Name);
            Assert.AreEqual(updatedComic.Notes, retrievedComic.Notes);

            Assert.IsNotNull(retrievedComic.Publisher);
            Assert.AreEqual(updatedComic.Publisher.Name, retrievedComic.Publisher.Name);

            Assert.IsNotNull(retrievedComic.Issues);
            Assert.AreEqual(1, retrievedComic.Issues.Count);

            ComicBookIssue retrievedIssue = retrievedComic.Issues[0];
            Assert.AreEqual(retrievedComic.Id, retrievedIssue.ComicBookId);
            AssertEquality(updatedComic.Issues[0], retrievedIssue);

            // IDs should not change
            Assert.AreEqual(comicId, retrievedComic.Id);
            Assert.AreEqual(comicIssueId, retrievedComic.Issues[0].Id);
            Assert.AreEqual(comicId, retrievedComic.Issues[0].ComicBookId);
        }

        [Test]
        public void UpdateCreatesIssueTest()
        {
            ComicBook comic = new ComicBook()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryCategory
            };

            comic.Issues.Add(new ComicBookIssue()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                IssueNumber = "Test Issue",
                IssueType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            });

            comicPersistence.Create(comic);
            long comicId = comic.Id;
            long initialIssueId = comic.Issues[0].Id;

            ComicBookIssue updatedIssue = new ComicBookIssue()
            {
                Id = initialIssueId,
                ComicBookId = comicId,
                Condition = "Test Condition 2",
                Cover = "Test Cover 2",
                DistributionType = DistributionType.Physical,
                IssueNumber = "Test Issue 2",
                IssueType = VolumeType.TPB,
                Name = "Title 2",
                Notes = "Test Notes 2"
            };

            ComicBookIssue issueNewToUpdate = new ComicBookIssue()
            {
                Condition = "Test Condition 3",
                Cover = "Test Cover 3",
                DistributionType = DistributionType.Digital,
                IssueNumber = "Test Issue 3",
                IssueType = VolumeType.Issue,
                Name = "Title 3",
                Notes = "Test Notes 3"
            };

            comic.Issues.Clear();
            comic.Issues.Add(updatedIssue);
            comic.Issues.Add(issueNewToUpdate);
            comicPersistence.Update(comic);

            ComicBook retrievedBook = comicPersistence.Retrieve(comicId);
            Assert.IsNotNull(retrievedBook);
            Assert.AreEqual(comicId, retrievedBook.Id);
            Assert.IsNotNull(retrievedBook.Issues);
            Assert.AreEqual(2, retrievedBook.Issues.Count);

            ComicBookIssue initialRetrievedIssue = retrievedBook.Issues
                .Single(i => i.Id == updatedIssue.Id);

            Assert.AreEqual(retrievedBook.Id, initialRetrievedIssue.ComicBookId);
            AssertEquality(updatedIssue, initialRetrievedIssue);

            ComicBookIssue secondRetrievedIssue = retrievedBook.Issues
                .Single(i => i.Id == issueNewToUpdate.Id);

            Assert.AreEqual(retrievedBook.Id, secondRetrievedIssue.ComicBookId);
            AssertEquality(issueNewToUpdate, secondRetrievedIssue);
        }

        [Test]
        public void UpdateDeletesIssueTest()
        {
            ComicBook comic = new ComicBook()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryCategory
            };

            ComicBookIssue firstIssue = new ComicBookIssue()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                IssueNumber = "Test Issue",
                IssueType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            };

            ComicBookIssue issueToDelete = new ComicBookIssue()
            {
                Condition = "Test Condition",
                Cover = "Test Cover",
                DistributionType = DistributionType.Digital,
                IssueNumber = "Test Issue",
                IssueType = VolumeType.Issue,
                Name = "Title",
                Notes = "Test Notes"
            };

            comic.Issues.Add(firstIssue);
            comic.Issues.Add(issueToDelete);

            comicPersistence.Create(comic);

            comic.Issues.Remove(firstIssue);

            comicPersistence.Update(comic);

            ComicBook retrievedBook = comicPersistence.Retrieve(comic.Id);
            Assert.IsNotNull(retrievedBook);
            Assert.IsNotNull(retrievedBook.Issues);
            Assert.AreEqual(1, retrievedBook.Issues.Count);
            Assert.IsFalse(retrievedBook.Issues.Any(i => i.Id == firstIssue.Id));
            Assert.IsTrue(retrievedBook.Issues.Any(i => i.Id == issueToDelete.Id));
        }

        [Test]
        public void DeleteTest()
        {
            ComicBook newComic = new ComicBook()
            {
                Name = "Test Book",
                Notes = "Test Notes",
                Publisher = primaryCategory
            };
            
            newComic.Issues.Add(new ComicBookIssue());

            comicPersistence.Create(newComic);
            comicPersistence.Delete(newComic);

            Assert.IsNull(comicPersistence.Retrieve(newComic.Id));
        }

        [Test]
        public void PaginationTest()
        {
            #region Test Setup
            if (File.Exists(Path.Combine("pagination", "collection.db")))
            {
                File.Delete(Path.Combine("pagination", "collection.db"));
            }

            MigrationRunner.Run(".", "pagination");

            IComicBookPersistence target =
                new ComicBookPersistence(".", "pagination");

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(".", "pagination");

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

            List<ComicBook> books = new List<ComicBook>();
            books.Add(new ComicBook()
            {
                Name = "Test Book 1",
                Notes = "Test Notes",
                Publisher = categories[0]
            });

            books.Add(new ComicBook()
            {
                Name = "Test Book 2",
                Notes = "Test Notes",
                Publisher = categories[0]
            });

            books.Add(new ComicBook()
            {
                Name = "Test Book 3",
                Notes = "Test Notes",
                Publisher = categories[0]
            });

            books.Add(new ComicBook()
            {
                Name = "Test Book 4",
                Notes = "Test Notes",
                Publisher = categories[1]
            });

            foreach (ComicBook book in books)
            {
                // Add issues to each book before adding the book
                book.Issues.Add(new ComicBookIssue()
                {
                    IssueNumber = "1",
                    ListType = ItemListType.Have
                });
                
                book.Issues.Add(new ComicBookIssue()
                {
                    IssueNumber = "2",
                    ListType = ItemListType.Have
                });
                
                book.Issues.Add(new ComicBookIssue()
                {
                    IssueNumber = "3",
                    ListType = ItemListType.Want
                });
                
                target.Create(book);
            }

            #endregion

            List<ComicBookSummary> tempResults = new List<ComicBookSummary>();

            // Null Category
            ModelSearchOptions allInclusiveOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                AllListTypes = true
            }.Build();

            Assert.AreEqual(4, target.TotalResults(allInclusiveOptions));

            for (long pageNumber = 0; pageNumber < 4; ++pageNumber)
            {
                List<ComicBookSummary> currentPage = target.Page(allInclusiveOptions, pageNumber);
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
                ItemCategory = categories[0],
                AllListTypes = true
            }.Build();

            Assert.AreEqual(3, target.TotalResults(categoryOneOptions));

            for (long pageNumber = 0; pageNumber < 3; ++pageNumber)
            {
                List<ComicBookSummary> currentPage = target.Page(categoryOneOptions, pageNumber);
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
                SearchText = books[0].Name,
                AllListTypes = true
            }.Build();

            tempResults = target.Page(nameSearch, 0);
            Assert.AreEqual(1, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == books[0].Id));
        }

        private static void AssertEquality(ComicBookIssue expected, ComicBookIssue actual)
        {
            Assert.AreEqual(expected.Condition, actual.Condition);
            Assert.AreEqual(expected.Cover, actual.Cover);
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.IssueNumber, actual.IssueNumber);
            Assert.AreEqual(expected.IssueType, actual.IssueType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Notes, actual.Notes);
            Assert.AreEqual(expected.ListType, actual.ListType);
        }
    }
}
