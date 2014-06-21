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
    public sealed class ProductPersistenceTests
    {
        private ISearchablePersistence<Product> productPersistence;
        private ItemCategory primaryCategory;
        private ItemCategory secondaryCategory;

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

            productPersistence = new ProductPersistence(currentDir, defaultProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, defaultProfile);

            primaryCategory = new ItemCategory()
            {
                Name = "Goods",
                CategoryType = ItemCategoryType.Product
            };

            categoryPersistence.Create(primaryCategory);
            
            secondaryCategory = new ItemCategory()
            {
                Name = "Goods 2",
                CategoryType = ItemCategoryType.Product
            };
            
            categoryPersistence.Create(secondaryCategory);
        }

        [Test]
        public void CreateTest()
        {
            Product newProduct = new Product
            {
                Name = "Test",
                Notes = "Test Notes",
                Quantity = 1,
                Category = primaryCategory
            };

            productPersistence.Create(newProduct);

            Product retrievedProduct = productPersistence.Retrieve(newProduct.Id);
            Assert.IsNotNull(retrievedProduct);

            AssertEquality.For(newProduct, retrievedProduct);
        }

        [Test]
        public void UpdateTest()
        {
            Product newProduct = new Product
            {
                Name = "Test",
                Notes = "Test Notes",
                Quantity = 1,
                Category = primaryCategory
            };

            productPersistence.Create(newProduct);

            Product updatedProduct = new Product
            {
                Id = newProduct.Id,
                Name = "Test2",
                Notes = "Test Notes 2",
                Quantity = 5,
                Category = secondaryCategory
            };

            productPersistence.Update(updatedProduct);

            Product retrievedProduct = productPersistence
                .Retrieve(updatedProduct.Id);

            Assert.IsNotNull(retrievedProduct);
            AssertEquality.For(updatedProduct, retrievedProduct);
        }

        [Test]
        public void DeleteTest()
        {
            Product newProduct = new Product
            {
                Name = "Test",
                Notes = "Test Notes",
                Quantity = 1,
                Category = primaryCategory
            };

            productPersistence.Create(newProduct);
            productPersistence.Delete(newProduct);

            Assert.IsNull(productPersistence.Retrieve(newProduct.Id));
        }
        
        [Test]
        public void RetrieveNullTest()
        {
            Assert.IsNull(productPersistence.Retrieve(long.MaxValue));
        }
        
        [TestCase(long.MinValue)]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [ExpectedException(typeof(ArgumentException))]
        public void RetrieveInvalidTest(long invalidId)
        {
            Assert.IsNull(productPersistence.Retrieve(invalidId));
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNullTest()
        {
            productPersistence.Create(null);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullTest()
        {
            productPersistence.Update(null);
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

            ISearchablePersistence<Product> target =
                new ProductPersistence(currentDir, paginationProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, paginationProfile);

            List<ItemCategory> categories = new List<ItemCategory>()
            {
                new ItemCategory()
                {
                    Name = "Category 1",
                    CategoryType = ItemCategoryType.Product,
                    Code = "001"
                },

                new ItemCategory()
                {
                    Name = "Category 2",
                    CategoryType = ItemCategoryType.Product,
                    Code = "002"
                }
            };

            foreach (ItemCategory category in categories)
            {
                categoryPersistence.Create(category);
            }

            List<Product> products = new List<Product>();
            products.Add(new Product()
            {
                Name = "Test Book 1",
                Notes = "Test Notes",
                Quantity = 1,
                Category = categories[0],
                ListType = ItemListType.Want
            });

            products.Add(new Product()
            {
                Name = "Test Book 2",
                Notes = "Test Notes",
                Quantity = 2,
                Category = categories[0],
                ListType = ItemListType.Want
            });

            products.Add(new Product()
            {
                Name = "Test Item 3",
                Notes = "Test Notes",
                Quantity = 3,
                Category = categories[0],
                ListType = ItemListType.Want
            });

            products.Add(new Product()
            {
                Name = "Test Item 4",
                Notes = "Test Notes",
                Quantity = 4,
                Category = categories[1],
                ListType = ItemListType.Want
            });

            foreach (Product book in products)
            {
                target.Create(book);
            }

            #endregion

            List<Product> tempResults = new List<Product>();

            // Null Category
            ModelSearchOptions allInclusiveOptions = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                ListType = ItemListType.Want
            }.Build();

            Assert.AreEqual(4, target.TotalResults(allInclusiveOptions));

            for (long pageNumber = 0; pageNumber < 4; ++pageNumber)
            {
                List<Product> currentPage = target.Page(allInclusiveOptions, pageNumber);
                Assert.AreEqual(1, currentPage.Count);
                tempResults.AddRange(currentPage);
            }

            Assert.AreEqual(4, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == products[0].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == products[1].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == products[2].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == products[3].Id));

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
                List<Product> currentPage = target.Page(categoryOneOptions, pageNumber);
                Assert.AreEqual(1, currentPage.Count);
                tempResults.AddRange(currentPage);
            }

            Assert.AreEqual(3, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == products[0].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == products[1].Id));
            Assert.IsTrue(tempResults.Any(i => i.Id == products[2].Id));
            Assert.IsFalse(tempResults.Any(i => i.Id == products[3].Id)); // has 'category 2'

            // Name Search
            ModelSearchOptions nameSearch = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 1,
                SearchText = products[0].Name
            }.Build();

            tempResults = target.Page(nameSearch, 0);
            Assert.AreEqual(1, tempResults.Count);
            Assert.IsTrue(tempResults.Any(i => i.Id == products[0].Id));
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

            ISearchablePersistence<Product> target =
                new ProductPersistence(currentDir, listProfile);

            IPersistence<ItemCategory> categoryPersistence =
                new ItemCategoryPersistence(currentDir, listProfile);

            ItemCategory category = categoryPersistence.Create(new ItemCategory()
            {
                Name = "Category 1",
                CategoryType = ItemCategoryType.Product,
                Code = "001"
            });


            foreach (ItemListType listType in Enum.GetValues(typeof(ItemListType)).Cast<ItemListType>())
            {
                target.Create(new Product()
                {
                    Name = string.Format("Test Product {0}", listType),
                    Quantity = 1,
                    ListType = listType,
                    Category = category
                });
            }

            // Create an additional 'Have' product
            target.Create(new Product()
            {
                Name = string.Format("Test Product {0} 2", ItemListType.Have),
                Quantity = 1,
                ListType = ItemListType.Have,
                Category = category
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
