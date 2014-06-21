// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Persistence.Export;
using LeavinsSoftware.Collection.Persistence.Export.Extensions;
using LeavinsSoftware.Collection.Persistence;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LeavinsSoftware.Collection.Persistence.Migrations;
using LeavinsSoftware.Collection.Models;
using System.Globalization;
using LeavinsSoftware.Collection.Tests.Helpers;
namespace LeavinsSoftware.Collection.Tests.Persistence.Export
{
    [TestFixture]
    public sealed class PersistenceImporterTests
    {
        private const string MainImportFileName = "Files\\Import Data 0.xml";
        private PersistenceImporter target;
        private ISearchablePersistence<ComicBookSeries> comicBookPersistence;
        private ISearchablePersistence<Product> productPersistence;
        private ICategoryPersistence categoryPersistence;
        private ISearchablePersistence<VideoGame> videoGamePersistence;

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(Path.Combine("default", "collection.db")))
            {
                File.Delete(Path.Combine("default", "collection.db"));
            }

            DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            Profile defaultProfile = new Profile("default");
            MigrationRunner.Run(currentDir, defaultProfile);

            comicBookPersistence = new ComicBookPersistence(currentDir, defaultProfile);
            productPersistence = new ProductPersistence(currentDir, defaultProfile);
            categoryPersistence = new ItemCategoryPersistence(currentDir, defaultProfile);
            videoGamePersistence = new VideoGamePersistence(currentDir, defaultProfile);

            target = PersistenceImporter
                .New()
                .CategoryPersistence(categoryPersistence)
                .ComicBookPersistence(comicBookPersistence)
                .ProductPersistence(productPersistence)
                .VideoGamePersistence(videoGamePersistence)
                .Build();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportTest(bool merge)
        {
            target.Import(MainImportFileName, new ImportOptions(merge: merge));

            ModelSearchOptions searchOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20
            }.Build();

            // ComicBook checks
            List<ComicBookSeries> comicResults = comicBookPersistence.Page(searchOptions, 0);
            Assert.IsNotNull(comicResults);
            Assert.AreEqual(2, comicResults.Count);
            Assert.AreEqual(3, comicResults.First().EntriesCount);
            Assert.AreEqual(3, comicResults.Skip(1).First().EntriesCount);

            foreach (ComicBookSeries summary in comicResults)
            {
                ComicBookSeries actualBook = comicBookPersistence.Retrieve(summary.Id);
                Assert.IsNotNull(actualBook);
                Assert.AreEqual(3, actualBook.Entries.Count);
            }

            // Product checks
            List<Product> productResults = productPersistence.Page(searchOptions, 0);
            Assert.IsNotNull(productResults);
            Assert.AreEqual(4, productResults.Count);

            // Video game checks
            List<VideoGame> gameResults = videoGamePersistence.Page(searchOptions, 0);
            Assert.IsNotNull(gameResults);
            Assert.AreEqual(4, gameResults.Count);

            #region Individual ComicBookSeries Checks

            ComicBookSeries[] actualBooks = new ComicBookSeries[]
            {
                comicBookPersistence.Retrieve(1),
                comicBookPersistence.Retrieve(2)
            };

            Assert.IsNotNull(actualBooks[0]);
            Assert.AreEqual("Test Book #1938005744", actualBooks[0].Name);
            Assert.AreEqual("Notes", actualBooks[0].Notes);
            Assert.AreEqual("ComicPublisher0", actualBooks[0].Publisher.Name);

            ComicBookSeriesEntry tempActualEntry = actualBooks[0].Entries
                .SingleOrDefault(c => c.Number == "2080427802");

            Assert.IsNotNull(tempActualEntry, "Cannot find issue 2080427802");
            Assert.AreEqual("Cover", tempActualEntry.Cover);
            Assert.AreEqual("Entry Name", tempActualEntry.Name);
            Assert.AreEqual("Good", tempActualEntry.Condition);
            Assert.AreEqual("Entry Notes", tempActualEntry.Notes);
            Assert.AreEqual(DistributionType.Physical, tempActualEntry.DistributionType);
            Assert.AreEqual(ItemListType.Have, tempActualEntry.ListType);
            Assert.AreEqual(VolumeType.Issue, tempActualEntry.EntryType);

            tempActualEntry = actualBooks[0].Entries
                .SingleOrDefault(c => c.Number == "1431988776");

            Assert.IsNotNull(tempActualEntry, "Cannot find issue 1431988776");
            Assert.AreEqual(ItemListType.Want, tempActualEntry.ListType);
            Assert.AreEqual(VolumeType.TPB, tempActualEntry.EntryType);
            Assert.AreEqual(DistributionType.Digital, tempActualEntry.DistributionType);

            tempActualEntry = actualBooks[0].Entries
                .SingleOrDefault(c => c.Number == "341851734");
            Assert.IsNotNull(tempActualEntry, "Cannot find issue 341851734");

            #endregion

            #region Individual Product Checks

            Product[] actualProducts = new Product[]
            {
                productPersistence.Retrieve(1),
                productPersistence.Retrieve(2)
            };

            Assert.IsNotNull(actualProducts[0]);
            Assert.AreEqual("Item #749943798", actualProducts[0].Name);
            Assert.AreEqual("Notes", actualProducts[0].Notes);
            Assert.AreEqual("ProductCategory0", actualProducts[0].Category.Name);
            Assert.AreEqual(5, actualProducts[0].Quantity);
            Assert.AreEqual(ItemListType.Have, actualProducts[0].ListType);

            Assert.IsNotNull(actualProducts[1]);
            Assert.AreEqual(ItemListType.Want, actualProducts[1].ListType);

            #endregion

            #region Individual VideoGame Checks

            VideoGame[] actualGames = new VideoGame[]
            {
                videoGamePersistence.Retrieve(1),
                videoGamePersistence.Retrieve(2)
            };

            Assert.IsNotNull(actualGames[0]);
            Assert.AreEqual("Game #811244070", actualGames[0].Name);
            Assert.AreEqual("Notes", actualGames[0].Notes);
            Assert.AreEqual("System0", actualGames[0].System.Name);
            Assert.AreEqual(DistributionType.Physical, actualGames[0].DistributionType);
            Assert.AreEqual("Good", actualGames[0].Condition);
            Assert.IsTrue(string.IsNullOrEmpty(actualGames[0].ContentProvider));
            Assert.AreEqual(ItemListType.Have, actualGames[0].ListType);

            Assert.IsNotNull(actualGames[1]);
            Assert.AreEqual("Game #1063087737", actualGames[1].Name);
            Assert.AreEqual("Online Service", actualGames[1].ContentProvider);
            Assert.IsTrue(string.IsNullOrEmpty(actualGames[1].Condition));
            Assert.AreEqual(ItemListType.Want, actualGames[1].ListType);

            #endregion
        }

        [Test]
        public void ImportMergeAllTest()
        {
            // SETUP
            target.Import(MainImportFileName, new ImportOptions(merge: true));

            List<ItemCategory> preImportCategories = new List<ItemCategory>();
            preImportCategories.AddRange(categoryPersistence.RetrieveAll(ItemCategoryType.ComicBook));
            preImportCategories.AddRange(categoryPersistence.RetrieveAll(ItemCategoryType.Product));
            preImportCategories.AddRange(categoryPersistence.RetrieveAll(ItemCategoryType.VideoGame));

            List<ComicBookSeries> preImportComics = new List<ComicBookSeries>();

            foreach (var page in comicBookPersistence.AllPages())
            {
                foreach (ComicBookSeries summary in page)
                {
                    preImportComics.Add(comicBookPersistence.Retrieve(summary.Id));
                }
            }

            List<Product> preImportProducts = new List<Product>();

            foreach (var page in productPersistence.AllPages())
            {
                preImportProducts.AddRange(page);
            }

            List<VideoGame> preImportGames = new List<VideoGame>();

            foreach (var page in videoGamePersistence.AllPages())
            {
                preImportGames.AddRange(page);
            }

            // TESTS - Try to import the same file again
            target.Import(MainImportFileName, new ImportOptions(merge: true));

            // Category checks
            foreach (ItemCategory postImportCategory in categoryPersistence.RetrieveAll(ItemCategoryType.ComicBook))
            {
                ItemCategory preImportMatch = preImportCategories
                    .SingleOrDefault(i => i.Id == postImportCategory.Id);

                Assert.IsNotNull(preImportMatch);
                Assert.AreEqual(preImportMatch, postImportCategory);
            }

            foreach (ItemCategory postImportCategory in categoryPersistence.RetrieveAll(ItemCategoryType.Product))
            {
                ItemCategory preImportMatch = preImportCategories
                    .SingleOrDefault(i => i.Id == postImportCategory.Id);

                Assert.IsNotNull(preImportMatch);
                Assert.AreEqual(preImportMatch, postImportCategory);
            }

            foreach (ItemCategory postImportCategory in categoryPersistence.RetrieveAll(ItemCategoryType.VideoGame))
            {
                ItemCategory preImportMatch = preImportCategories
                    .SingleOrDefault(i => i.Id == postImportCategory.Id);

                Assert.IsNotNull(preImportMatch);
                Assert.AreEqual(preImportMatch, postImportCategory);
            }

            // ComicBook checks
            foreach (var page in comicBookPersistence.AllPages())
            {
                foreach (ComicBookSeries summary in page)
                {
                    ComicBookSeries book = comicBookPersistence.Retrieve(summary.Id);
                    ComicBookSeries preImportMatch = preImportComics
                        .SingleOrDefault(c => c.Id == book.Id);

                    string msg = string.Format("book ID: {0}", book.Id);
                    Assert.IsNotNull(preImportMatch, msg);
                    AssertEquality.For(preImportMatch, book);
                }
            }

            // Product checks
            foreach (var page in productPersistence.AllPages())
            {
                foreach (Product product in page)
                {
                    Product preImportMatch = preImportProducts
                        .SingleOrDefault(c => c.Id == product.Id);

                    Assert.IsNotNull(preImportMatch, "ID: {0}", product.Id);
                    AssertEquality.For(preImportMatch, product);
                }
            }

            // VideoGame checks
            foreach (var page in videoGamePersistence.AllPages())
            {
                foreach (VideoGame game in page)
                {
                    VideoGame preImportMatch = preImportGames
                        .SingleOrDefault(c => c.Id == game.Id);

                    Assert.IsNotNull(preImportMatch, "ID: {0}", game.Id);
                    AssertEquality.For(preImportMatch, game);
                }
            }
        }
        
        [Test]
        public void ImportMergeUpdateTest()
        {
            // TODO: Update test file with additional changes (to be ignored by merge)
            // SETUP
            target.Import(MainImportFileName, new ImportOptions(merge: false));
            ComicBookSeries expectedComicSeries = comicBookPersistence.Retrieve(1);
            expectedComicSeries.Notes += "\nUpdate Notes";

            ComicBookSeriesEntry updatedEntry = expectedComicSeries.Entries
                .Single(e => e.Id == 1);
            updatedEntry.Notes += "\nUpdate Notes Entry";
            updatedEntry.ListType = ItemListType.Want;

            VideoGame expectedGame = videoGamePersistence.Retrieve(1);
            expectedGame.Notes += "\nUpdate Notes";
            expectedGame.ListType = ItemListType.Want;

            Product expectedProduct = productPersistence.Retrieve(1);
            expectedProduct.Notes += "\nUpdate Notes";
            expectedProduct.ListType = ItemListType.Want;

            // Test
            target.Import("Files\\Import Data Update.xml", new ImportOptions(merge: true));
            
            AssertEquality.For(expectedComicSeries, comicBookPersistence.Retrieve(1));
            AssertEquality.For(expectedGame, videoGamePersistence.Retrieve(1));
            AssertEquality.For(expectedProduct, productPersistence.Retrieve(1));
        }
    }
}
