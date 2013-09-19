// Copyright (c) 2013 Dustin Leavins
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
namespace LeavinsSoftware.Collection.Tests.Persistence.Export
{
    [TestFixture]
    public sealed class PersistenceImporterTests
    {
        private PersistenceImporter target;
        private IComicBookPersistence comicBookPersistence;
        private IProductPersistence productPersistence;
        private ICategoryPersistence categoryPersistence;
        private IVideoGamePersistence videoGamePersistence;

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

        [Test]
        public void ImportTest()
        {
            const string fileName = "Files\\Import Data 0.xml";
            target.Import(fileName, new ImportOptions(merge: false));

            ModelSearchOptions searchOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20,
                AllListTypes = true
            }.Build();

            // ComicBook checks
            List<ComicBookSummary> comicResults = comicBookPersistence.Page(searchOptions, 0);
            Assert.IsNotNull(comicResults);
            Assert.AreEqual(2, comicResults.Count);
            Assert.AreEqual(2, comicResults.First().IssueCount);
            Assert.AreEqual(2, comicResults.Skip(1).First().IssueCount);

            foreach (ComicBookSummary summary in comicResults)
            {
                ComicBook actualBook = comicBookPersistence.Retrieve(summary.Id);
                Assert.IsNotNull(actualBook);
                Assert.AreEqual(3, actualBook.Issues.Count);
            }

            // Product checks
            List<Product> productResults = productPersistence.Page(searchOptions, 0);
            Assert.IsNotNull(productResults);
            Assert.AreEqual(4, productResults.Count);

            // Video game checks
            List<VideoGame> gameResults = videoGamePersistence.Page(searchOptions, 0);
            Assert.IsNotNull(gameResults);
            Assert.AreEqual(4, gameResults.Count);
        }
        
        [Test]
        public void ImportMergeAllTest()
        {
            // SETUP
            const string fileName = "Files\\Import Data 0.xml";
            target.Import(fileName, new ImportOptions(merge: true ));
            
            List<ItemCategory> preImportCategories = new List<ItemCategory>();
            preImportCategories.AddRange(categoryPersistence.RetrieveAll(ItemCategoryType.ComicBook));
            preImportCategories.AddRange(categoryPersistence.RetrieveAll(ItemCategoryType.Product));
            preImportCategories.AddRange(categoryPersistence.RetrieveAll(ItemCategoryType.VideoGame));

            List<ComicBook> preImportComics = new List<ComicBook>();

            foreach (var page in comicBookPersistence.AllPages())
            {
                foreach (ComicBookSummary summary in page)
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
            target.Import(fileName, new ImportOptions(merge: true));

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
            
            // TODO: Check Item Equality

            // ComicBook checks
            foreach (var page in comicBookPersistence.AllPages())
            {
                foreach (ComicBookSummary summary in page)
                {
                    ComicBook book = comicBookPersistence.Retrieve(summary.Id);
                    ComicBook preImportMatch = preImportComics
                        .SingleOrDefault(c => c.Id == book.Id);

                    Assert.IsNotNull(preImportMatch, "ID: {0}", book.Id);
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
                }
            }
        }
        
        // TODO: Check what happens when import w/ new data is merged
    }
}
