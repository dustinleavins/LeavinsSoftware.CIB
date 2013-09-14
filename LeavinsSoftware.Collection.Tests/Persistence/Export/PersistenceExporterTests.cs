﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Persistence.Export;
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
    public sealed class PersistenceExporterTests
    {
        private PersistenceExporter target;
        private IComicBookPersistence comicBookPersistence;
        private IProductPersistence productPersistence;
        private ICategoryPersistence categoryPersistence;
        private IVideoGamePersistence videoGamePersistence;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            if (File.Exists(Path.Combine("default", "collection.db")))
            {
                File.Delete(Path.Combine("default", "collection.db"));
            }

            MigrationRunner.Run(".", "default");
            comicBookPersistence = new ComicBookPersistence(".", "default");
            productPersistence = new ProductPersistence(".", "default");
            categoryPersistence = new ItemCategoryPersistence(".", "default");
            videoGamePersistence = new VideoGamePersistence(".", "default");

            target = PersistenceExporter
                .New()
                .ComicBookPersistence(comicBookPersistence)
                .ProductPersistence(productPersistence)
                .VideoGamePersistence(videoGamePersistence)
                .Build();

            Random rnd = new Random(100);

            #region Setup Comic Book Persistence

            // Publishers
            ItemCategory[] comicPublishers =
            {
                categoryPersistence.Create(new ItemCategory()
                {
                    Name = "ComicPublisher0",
                    CategoryType = ItemCategoryType.ComicBook
                }),

                categoryPersistence.Create(new ItemCategory()
                {
                    Name = "ComicPublisher1",
                    CategoryType = ItemCategoryType.ComicBook
                })
            };

            // Comics
            comicBookPersistence.Create(GenerateComic(rnd, comicPublishers[0],
                GenerateIssue(rnd, ItemListType.Have),
                GenerateIssue(rnd, ItemListType.Have),
                GenerateIssue(rnd, ItemListType.Want)));

            comicBookPersistence.Create(GenerateComic(rnd, comicPublishers[1],
                GenerateIssue(rnd, ItemListType.Have),
                GenerateIssue(rnd, ItemListType.Have),
                GenerateIssue(rnd, ItemListType.Want)));

            #endregion

            #region Setup Product Persistence
            // Categories
            ItemCategory[] productCategories =
            {
                categoryPersistence.Create(new ItemCategory()
                {
                    Name = "ProductCategory0",
                    CategoryType = ItemCategoryType.Product
                }),
                categoryPersistence.Create(new ItemCategory()
                {
                    Name = "ProductCategory1",
                    CategoryType = ItemCategoryType.Product
                })
            };

            productPersistence.Create(GenerateProduct(rnd,
                productCategories[0],
                ItemListType.Have));

            productPersistence.Create(GenerateProduct(rnd,
                productCategories[0],
                ItemListType.Want));

            productPersistence.Create(GenerateProduct(rnd,
                productCategories[1],
                ItemListType.Have));

            productPersistence.Create(GenerateProduct(rnd,
                productCategories[1],
                ItemListType.Want));

            #endregion

            #region Setup Video Game Persistence

            ItemCategory[] gameSystems =
            {
                categoryPersistence.Create(new ItemCategory()
                {
                    Name = "System0",
                    CategoryType = ItemCategoryType.VideoGame
                }),

                categoryPersistence.Create(new ItemCategory()
                {
                    Name = "System1",
                    CategoryType = ItemCategoryType.VideoGame
                })
            };

            videoGamePersistence.Create(GenerateVideoGame(rnd,
                gameSystems[0],
                ItemListType.Have));

            videoGamePersistence.Create(GenerateVideoGame(rnd,
                gameSystems[0],
                ItemListType.Want));

            videoGamePersistence.Create(GenerateVideoGame(rnd,
                gameSystems[1],
                ItemListType.Have));

            videoGamePersistence.Create(GenerateVideoGame(rnd,
                gameSystems[1],
                ItemListType.Want));

            #endregion
        }

        [Test]
        public void ExportTest()
        {
            const string fileName = "export_data.xml";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            target.Export(fileName);
            Assert.IsTrue(File.Exists("export_data.xml"));
        }

        private static ComicBook GenerateComic(Random rnd,
            ItemCategory publisher, params ComicBookIssue[] issues)
        {
            ComicBook book = new ComicBook()
            {
                Name = string.Format("Test Book #{0}", rnd.Next()),
                Publisher = publisher
            };

            foreach (ComicBookIssue issue in issues)
            {
                book.Issues.Add(issue);
            }

            return book;
        }

        private static ComicBookIssue GenerateIssue(Random rnd,
            ItemListType listType)
        {
            return new ComicBookIssue()
            {
                IssueNumber = rnd.Next(1, int.MaxValue).ToString(CultureInfo.InvariantCulture),
                ListType = listType
            };
        }

        private static Product GenerateProduct(Random rnd,
            ItemCategory category, ItemListType listType)
        {
            return new Product()
            {
                Name = string.Format(CultureInfo.InvariantCulture,
                    "Item #{0}",
                    rnd.Next(1, int.MaxValue)),

                Category = category,
                ListType = listType
            };
        }

        private static VideoGame GenerateVideoGame(Random rnd,
            ItemCategory system, ItemListType listType)
        {
            return new VideoGame()
            {
                Name = string.Format(CultureInfo.InvariantCulture,
                    "Game #{0}",
                    rnd.Next(1, int.MaxValue)),

                System = system,
                ListType = listType
            };
        }
    }
}