﻿// Copyright (c) 2013 Dustin Leavins
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
    public sealed class VideoGamePersistenceTests
    {
        private IVideoGamePersistence gamePersistence;
        private ItemCategory primaryCategory;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            if (File.Exists(Path.Combine("default", "collection.db")))
            {
                File.Delete(Path.Combine("default", "collection.db"));
            }
            
            MigrationRunner.Run(".", "default");
            gamePersistence = new VideoGamePersistence(".", "default");

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(".", "default");

            categoryPersistence.Create(new ItemCategory()
            {
                Name = "Goods",
            });

            primaryCategory = new ItemCategory()
            {
                Name = "Goods",
            };

            categoryPersistence.Create(primaryCategory);
        }

        [Test]
        public void CreateTest()
        {
            VideoGame newGame = new VideoGame
            {
                Name = "Test Game",
                Notes = "Test Notes",
                ContentProvider = "Provider",
                Condition = "Condition",
                DistributionType = DistributionType.Physical,

                System = primaryCategory
            };

            gamePersistence.Create(newGame);

            VideoGame retrievedGame = gamePersistence.Retrieve(newGame.Id);
            Assert.IsNotNull(retrievedGame);

            AssertEquality(newGame, retrievedGame);
        }

        [Test]
        public void UpdateTest()
        {
            VideoGame newGame = new VideoGame
            {
                Name = "Test Game",
                Notes = "Test Notes",
                ContentProvider = "Provider",
                Condition = "Condition",
                DistributionType = DistributionType.Physical,

                System = primaryCategory
            };

            gamePersistence.Create(newGame);
            
            VideoGame updatedGame = new VideoGame()
            {
                Id = newGame.Id,
                Name = "Test Game 2",
                Notes = "Test Notes 2",
                ContentProvider = "Provider 2",
                Condition = "Condition 2",
                DistributionType = DistributionType.Digital,

                // TODO: Change system
                System = primaryCategory
            };
            
            gamePersistence.Update(updatedGame);
            
            VideoGame retrievedGame = gamePersistence
                .Retrieve(updatedGame.Id);
            
            Assert.IsNotNull(retrievedGame);
            AssertEquality(updatedGame, retrievedGame);
        }
        
        [Test]
        public void DeleteTest()
        {
            VideoGame newGame = new VideoGame
            {
                Name = "Test Game",
                Notes = "Test Notes",
                ContentProvider = "Provider",
                Condition = "Condition",
                DistributionType = DistributionType.Physical,

                System = primaryCategory
            };
            
            gamePersistence.Create(newGame);
            gamePersistence.Delete(newGame);
            
            Assert.IsNull(gamePersistence.Retrieve(newGame.Id));
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

            IVideoGamePersistence target =
                new VideoGamePersistence(".", "pagination");

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(".", "pagination");

            List<ItemCategory> categories = new List<ItemCategory>()
            {
                new ItemCategory()
                {
                    Name = "Category 1",
                    CategoryType = ItemCategoryType.VideoGame,
                    Code = "001"
                },

                new ItemCategory()
                {
                    Name = "Category 2",
                    CategoryType = ItemCategoryType.VideoGame,
                    Code = "002"
                }
            };

            foreach (ItemCategory category in categories)
            {
                categoryPersistence.Create(category);
            }

            List<VideoGame> games = new List<VideoGame>();
            games.Add(new VideoGame()
            {
                Name = "Test Book 1",
                Notes = "Test Notes",
                ContentProvider = "Provider 2",
                Condition = "Condition 2",
                DistributionType = DistributionType.Digital,
                System = categories[0],
                ListType = ItemListType.Want
            });

            games.Add(new VideoGame()
            {
                Name = "Test Book 2",
                Notes = "Test Notes",
                ContentProvider = "Provider 2",
                Condition = "Condition 2",
                DistributionType = DistributionType.Digital,
                System = categories[0],
                ListType = ItemListType.Want
            });

            games.Add(new VideoGame()
            {
                Name = "Test Item 3",
                Notes = "Test Notes",
                ContentProvider = "Provider 2",
                Condition = "Condition 2",
                DistributionType = DistributionType.Digital,
                System = categories[0],
                ListType = ItemListType.Want
            });

            games.Add(new VideoGame()
            {
                Name = "Test Item 4",
                Notes = "Test Notes",
                ContentProvider = "Provider 2",
                Condition = "Condition 2",
                DistributionType = DistributionType.Digital,
                System = categories[1],
                ListType = ItemListType.Want
            });

            foreach (VideoGame book in games)
            {
                target.Create(book);
            }

            #endregion

            List<VideoGame> tempResults = new List<VideoGame>();

            // Null Category
            ModelSearchOptions allInclusiveOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                ListType = ItemListType.Want
            }.Build();

            Assert.AreEqual(4, target.TotalResults(allInclusiveOptions));

            for (long pageNumber = 0; pageNumber < 4; ++pageNumber)
            {
                List<VideoGame> currentPage = target.Page(allInclusiveOptions, pageNumber);
                Assert.AreEqual(1, currentPage.Count);
                tempResults.AddRange(currentPage);
            }

            Assert.AreEqual(4, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == games[0].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == games[1].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == games[2].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == games[3].Id));

            // Non-Null Category
            tempResults.Clear();

            ModelSearchOptions categoryOneOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                ItemCategory = categories[0],
                ListType = ItemListType.Want
            }.Build();

            Assert.AreEqual(3, target.TotalResults(categoryOneOptions));

            for (long pageNumber = 0; pageNumber < 3; ++pageNumber)
            {
                List<VideoGame> currentPage = target.Page(categoryOneOptions, pageNumber);
                Assert.AreEqual(1, currentPage.Count);
                tempResults.AddRange(currentPage);
            }

            Assert.AreEqual(3, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == games[0].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == games[1].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == games[2].Id));
            Assert.IsFalse(tempResults.Any(i => i.Id == games[3].Id)); // has 'category 2'

            // Name Search
            ModelSearchOptions nameSearch = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                SearchText = games[0].Name,
                AllListTypes = true
            }.Build();

            tempResults = target.Page(nameSearch, 0);
            Assert.AreEqual(1, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == games[0].Id));
        }

        private static void AssertEquality(VideoGame expected, VideoGame actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Notes, actual.Notes);
            Assert.AreEqual(expected.ContentProvider, actual.ContentProvider);
            Assert.AreEqual(expected.Condition, actual.Condition);
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.ListType, actual.ListType);

            Assert.IsNotNull(actual.System);
            Assert.AreEqual(expected.System.Name, actual.System.Name);
        }
    }
}
