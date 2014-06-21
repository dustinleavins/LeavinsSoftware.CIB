// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Migrations;
using LeavinsSoftware.Collection.Tests.Helpers;
using NUnit.Framework;

namespace LeavinsSoftware.Collection.Tests.Persistence
{
    [TestFixture]
    public sealed class VideoGamePersistenceTests
    {
        private ISearchablePersistence<VideoGame> gamePersistence;
        private ItemCategory primarySystem;
        private ItemCategory secondarySystem;

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
            gamePersistence = new VideoGamePersistence(currentDir, defaultProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, defaultProfile);

            primarySystem = new ItemCategory()
            {
                Name = "System 1",
                CategoryType = ItemCategoryType.VideoGame
            };

            categoryPersistence.Create(primarySystem);

            secondarySystem = new ItemCategory()
            {
                Name = "System 2",
                CategoryType = ItemCategoryType.VideoGame
            };

            categoryPersistence.Create(secondarySystem);
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

                System = primarySystem
            };

            gamePersistence.Create(newGame);

            VideoGame retrievedGame = gamePersistence.Retrieve(newGame.Id);
            Assert.IsNotNull(retrievedGame);

            AssertEquality.For(newGame, retrievedGame);
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

                System = primarySystem
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
                System = secondarySystem
            };
            
            gamePersistence.Update(updatedGame);
            
            VideoGame retrievedGame = gamePersistence
                .Retrieve(updatedGame.Id);
            
            Assert.IsNotNull(retrievedGame);
            AssertEquality.For(updatedGame, retrievedGame);
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

                System = primarySystem
            };
            
            gamePersistence.Create(newGame);
            gamePersistence.Delete(newGame);
            
            Assert.IsNull(gamePersistence.Retrieve(newGame.Id));
        }
        
        [Test]
        public void RetrieveNullTest()
        {
            Assert.IsNull(gamePersistence.Retrieve(long.MaxValue));
        }
        
        [TestCase(long.MinValue)]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [ExpectedException(typeof(ArgumentException))]
        public void RetrieveInvalidTest(long invalidId)
        {
            Assert.IsNull(gamePersistence.Retrieve(invalidId));
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNullTest()
        {
            gamePersistence.Create(null);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullTest()
        {
            gamePersistence.Update(null);
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
            

            ISearchablePersistence<VideoGame> target =
                new VideoGamePersistence(currentDir, paginationProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, paginationProfile);

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
                SearchText = games[0].Name
            }.Build();

            tempResults = target.Page(nameSearch, 0);
            Assert.AreEqual(1, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == games[0].Id));
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

            ISearchablePersistence<VideoGame> target =
                new VideoGamePersistence(currentDir, listProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, listProfile);

            ItemCategory category = categoryPersistence.Create(new ItemCategory()
            {
                Name = "Category 1",
                CategoryType = ItemCategoryType.VideoGame,
                Code = "001"
            });


            foreach (ItemListType listType in Enum.GetValues(typeof(ItemListType)).Cast<ItemListType>())
            {
                target.Create(new VideoGame()
                {
                    Name = string.Format("Test Product {0}", listType),
                    ListType = listType,
                    System = category
                });
            }

            // Create an additional 'Have' product
            target.Create(new VideoGame()
            {
                Name = string.Format("Test Product {0} 2", ItemListType.Have),
                System = category,
                ListType = ItemListType.Have
            });

            #endregion

            foreach (ItemListType listType in Enum.GetValues(typeof(ItemListType)).Cast<ItemListType>())
            {
                ModelSearchOptions searchOptions = new ModelSearchOptionsBuilder()
                {
                    ListType = listType,
                    ItemsPerPage = 20
                }.Build();

                string errorMsg = string.Format("Type: {0}", listType);

                if (listType == ItemListType.Have)
                {
                    Assert.AreEqual(2, target.TotalResults(searchOptions), errorMsg);
                    Assert.AreEqual(2, target.Page(searchOptions, 0).Count, errorMsg);
                }
                else
                {
                    Assert.AreEqual(1, target.TotalResults(searchOptions), errorMsg);
                    Assert.AreEqual(1, target.Page(searchOptions, 0).Count, errorMsg);
                }
            }
            
            // All-inclusive search
            ModelSearchOptions allInclusiveOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20
            }.Build();
            
            Assert.AreEqual(6, target.TotalResults(allInclusiveOptions));
            Assert.AreEqual(6, target.Page(allInclusiveOptions, 0).Count);
            
            // Name search
            ModelSearchOptions nameOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20,
                SearchText = "Test"
            }.Build();
            
            Assert.AreEqual(6, target.TotalResults(nameOptions));
            Assert.AreEqual(6, target.Page(nameOptions, 0).Count);
            
            // Name not found search
            ModelSearchOptions nameNotFoundOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 20,
                SearchText = "Not Found"
            }.Build();
            
            Assert.AreEqual(0, target.TotalResults(nameNotFoundOptions));
            Assert.AreEqual(0, target.Page(nameNotFoundOptions, 0).Count);
        }
    }
}
