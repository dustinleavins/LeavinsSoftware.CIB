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
    public sealed class ItemCategoryPersistenceTests
    {
        private ICategoryPersistence categoryPersistence;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            if (File.Exists(Path.Combine("default", "collection.db")))
            {
                File.Delete(Path.Combine("default", "collection.db"));
            }

            MigrationRunner.Run(".", "default");
            File.Copy(Path.Combine("default", "collection.db"), "collection.db.bak");

            categoryPersistence = new ItemCategoryPersistence(".", "default");
        }
        
        [SetUp]
        public void SetUp()
        {
            File.Copy("collection.db.bak",
                Path.Combine("default", "collection.db"),
                true);
        }

        [Test]
        public void CreateTest()
        {
            ItemCategory newCategory = new ItemCategory()
            {
                Name = "Test Category",
                Code = "C01",
                CategoryType = ItemCategoryType.ComicBook
            };

            categoryPersistence.Create(newCategory);

            ItemCategory retrievedCategory = categoryPersistence.Retrieve(newCategory.Id);
            Assert.IsNotNull(retrievedCategory);

            AssertEquality(newCategory, retrievedCategory);
        }

        [Test]
        public void UpdateTest()
        {
            ItemCategory newCategory = new ItemCategory()
            {
                Name = "Test Category",
                Code = "C01",
                CategoryType = ItemCategoryType.ComicBook
            };

            categoryPersistence.Create(newCategory);

            ItemCategory updatedCategory = new ItemCategory()
            {
                Id = newCategory.Id,
                Name = "Test Category 2",
                Code = "C05",
                CategoryType = ItemCategoryType.VideoGame
            };

            categoryPersistence.Update(updatedCategory);
            ItemCategory retrievedCategory = categoryPersistence.Retrieve(updatedCategory.Id);
            Assert.IsNotNull(retrievedCategory);

            AssertEquality(updatedCategory, retrievedCategory);
        }

        [Test]
        public void DeleteTest()
        {
            ItemCategory newCategory = new ItemCategory()
            {
                Name = "Test Category",
                Code = "C01",
                CategoryType = ItemCategoryType.ComicBook
            };

            categoryPersistence.Create(newCategory);
            categoryPersistence.Delete(newCategory);
            Assert.IsNull(categoryPersistence.Retrieve(newCategory.Id));
        }

        [Test]
        public void RetrieveAllTest()
        {
            ItemCategory newComicCategory = new ItemCategory()
            {
                CategoryType = ItemCategoryType.ComicBook,
                Name = "RetrieveAll Comic"
            };

            ItemCategory newProductCategory = new ItemCategory()
            {
                CategoryType = ItemCategoryType.Product,
                Name = "RetrieveAll Product"
            };

            ItemCategory newGameCategory = new ItemCategory()
            {
                CategoryType = ItemCategoryType.VideoGame,
                Name = "RetrieveAll Game"
            };

            categoryPersistence.Create(newComicCategory);
            categoryPersistence.Create(newProductCategory);
            categoryPersistence.Create(newGameCategory);

            // Comic Tests
            ICollection<ItemCategory> comicCategories =
                categoryPersistence.RetrieveAll(ItemCategoryType.ComicBook);

            Assert.IsNotNull(comicCategories);
            Assert.IsTrue(comicCategories.Any());
            Assert.IsFalse(comicCategories.Any(c => c.CategoryType != ItemCategoryType.ComicBook));

            Assert.AreEqual(1, comicCategories.Count(c => c.Id == newComicCategory.Id));
            
            // Product Tests
            ICollection<ItemCategory> productCategories =
                categoryPersistence.RetrieveAll(ItemCategoryType.Product);

            Assert.IsNotNull(productCategories);
            Assert.IsTrue(productCategories.Any());
            Assert.IsFalse(productCategories.Any(c => c.CategoryType != ItemCategoryType.Product));

            Assert.AreEqual(1, productCategories.Count(c => c.Id == newProductCategory.Id));
            
            // Game Tests
            ICollection<ItemCategory> gameCategories =
                categoryPersistence.RetrieveAll(ItemCategoryType.VideoGame);

            Assert.IsNotNull(gameCategories);
            Assert.IsTrue(gameCategories.Any());
            Assert.IsFalse(gameCategories.Any(c => c.CategoryType != ItemCategoryType.VideoGame));

            Assert.AreEqual(1, gameCategories.Count(c => c.Id == newGameCategory.Id));

            // All Tests
            ICollection<ItemCategory> allRetrievedCategories =
                categoryPersistence.RetrieveAll();

            Assert.IsNotNull(allRetrievedCategories);
            Assert.AreEqual(3, allRetrievedCategories.Count);
            Assert.AreEqual(1, allRetrievedCategories.Count(c => c.Id == newComicCategory.Id));
            Assert.AreEqual(1, allRetrievedCategories.Count(c => c.Id == newProductCategory.Id));
            Assert.AreEqual(1, allRetrievedCategories.Count(c => c.Id == newGameCategory.Id));
        }

        private static void AssertEquality(ItemCategory expected, ItemCategory actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.CategoryType, actual.CategoryType);
            Assert.AreEqual(expected.Code, actual.Code);
        }
    }
}
