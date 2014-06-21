// Copyright (c) 2013, 2014 Dustin Leavins
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
using SimpleInjector;

namespace LeavinsSoftware.Collection.Tests.Persistence.Export
{
    [TestFixture]
    public sealed class PersistenceExporterTests
    {
        private PersistenceExporter target;
        private ISearchablePersistence<ComicBookSeries> comicBookPersistence;
        private ISearchablePersistence<Product> productPersistence;
        private ICategoryPersistence categoryPersistence;
        private ISearchablePersistence<VideoGame> videoGamePersistence;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
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

            var container = new Container();
            container.RegisterSingle<ISearchablePersistence<ComicBookSeries>>(comicBookPersistence);
            container.RegisterSingle<ISearchablePersistence<Product>>(productPersistence);
            container.RegisterSingle<ISearchablePersistence<VideoGame>>(videoGamePersistence);
            container.RegisterSingle<ICategoryPersistence>(categoryPersistence);
            
            target = new PersistenceExporter(container);

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

        private static ComicBookSeries GenerateComic(Random rnd,
            ItemCategory publisher, params ComicBookSeriesEntry[] issues)
        {
            ComicBookSeries book = new ComicBookSeries()
            {
                Name = string.Format("Test Book #{0}", rnd.Next()),
                Publisher = publisher
            };

            foreach (ComicBookSeriesEntry issue in issues)
            {
                book.Entries.Add(issue);
            }

            return book;
        }

        private static ComicBookSeriesEntry GenerateIssue(Random rnd,
            ItemListType listType)
        {
            return new ComicBookSeriesEntry()
            {
                Number = rnd.Next(1, int.MaxValue).ToString(CultureInfo.InvariantCulture),
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
