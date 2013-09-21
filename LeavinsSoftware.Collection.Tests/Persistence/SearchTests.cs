using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace LeavinsSoftware.Collection.Tests.Persistence
{
    [TestFixture]
    public sealed class SearchTests
    {
        private Mock<ISearchablePersistence<Product>> persistenceMock;
        private ModelSearchOptions defaultOptions;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            defaultOptions = new ModelSearchOptionsBuilder()
            {
                AllListTypes = true,
                ItemCategory = null,
                ItemsPerPage = 20,
                ListType = ItemListType.Have, // unused
                SearchText = string.Empty
            }.Build();
        }

        [SetUp]
        public void SetUp()
        {
            persistenceMock = new Mock<ISearchablePersistence<Product>>();
        }

        [Test]
        public void ConstructorTest()
        {
            persistenceMock.Setup(p => p.Page(defaultOptions, 0))
                .Returns(GeneratePage(defaultOptions, 20));
            
            persistenceMock.Setup(p => p.TotalResults(defaultOptions))
                .Returns(20);

            Search<Product> target = new Search<Product>(persistenceMock.Object, defaultOptions);
            Assert.IsNotNull(target.Persistence);
            Assert.IsNotNull(target.CurrentPage);
            Assert.AreEqual(20, target.CurrentPage.Count);
            Assert.AreEqual(0, target.CurrentPageNumber);
            Assert.AreEqual(1, target.TotalNumberOfPages);
            Assert.IsFalse(target.HasPreviousPage);
            Assert.IsFalse(target.HasNextPage);
        }
        
        [Test]
        public void ConstructorNoResultsTest()
        {
            persistenceMock.Setup(p => p.TotalResults(defaultOptions))
                .Returns(0);

            Search<Product> target = new Search<Product>(persistenceMock.Object, defaultOptions);
            Assert.IsNotNull(target.Persistence);
            Assert.IsNotNull(target.CurrentPage);
            Assert.AreEqual(0, target.CurrentPage.Count);
            Assert.AreEqual(0, target.CurrentPageNumber);
            Assert.AreEqual(0, target.TotalNumberOfPages);
            Assert.IsFalse(target.HasPreviousPage);
            Assert.IsFalse(target.HasNextPage);
        }
        
        [Test]
        public void PaginationTest()
        {
            persistenceMock.Setup(p => p.Page(defaultOptions, 0))
                .Returns(GeneratePage(defaultOptions, 20));
            
            persistenceMock.Setup(p => p.Page(defaultOptions, 1))
                .Returns(GeneratePage(defaultOptions, 20));
            
            persistenceMock.Setup(p => p.Page(defaultOptions, 2))
                .Returns(GeneratePage(defaultOptions, 1));
            
            persistenceMock.Setup(p => p.TotalResults(defaultOptions))
                .Returns(41);

            Search<Product> target = new Search<Product>(persistenceMock.Object, defaultOptions);
            
            // Page 1
            Assert.IsFalse(target.HasPreviousPage);
            Assert.AreEqual(0, target.CurrentPageNumber);
            Assert.IsTrue(target.HasNextPage);
            
            // Page 2
            target.NextPage();
            Assert.AreEqual(1, target.CurrentPageNumber);
            Assert.AreEqual(20, target.CurrentPage.Count);
            Assert.IsTrue(target.HasPreviousPage);
            
            // Going back to first page...
            target.PreviousPage();
            Assert.AreEqual(0, target.CurrentPageNumber);
            
            // ...and back
            target.NextPage();
            Assert.AreEqual(1, target.CurrentPageNumber);
            Assert.IsTrue(target.HasNextPage);

            // Page 3
            target.NextPage();
            Assert.AreEqual(2, target.CurrentPageNumber);
            Assert.AreEqual(1, target.CurrentPage.Count);
            Assert.IsFalse(target.HasNextPage);
            
            Assert.IsTrue(target.HasPreviousPage);
            
            // Going back to page 2
            target.PreviousPage();
            Assert.AreEqual(1, target.CurrentPageNumber);
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextPageExceptionTest()
        {
            persistenceMock.Setup(p => p.Page(defaultOptions, 0))
                .Returns(GeneratePage(defaultOptions, 20));
            
            persistenceMock.Setup(p => p.TotalResults(defaultOptions))
                .Returns(20);

            Search<Product> target = new Search<Product>(persistenceMock.Object, defaultOptions);
            
            target.NextPage();
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreviousPageExceptionTest()
        {
            persistenceMock.Setup(p => p.Page(defaultOptions, 0))
                .Returns(GeneratePage(defaultOptions, 20));
            
            persistenceMock.Setup(p => p.TotalResults(defaultOptions))
                .Returns(20);

            Search<Product> target = new Search<Product>(persistenceMock.Object, defaultOptions);
            
            target.PreviousPage();
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(-1)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GoToPageExceptionTest(long pageNumber)
        {
            persistenceMock.Setup(p => p.Page(defaultOptions, 0))
                .Returns(GeneratePage(defaultOptions, 20));
            
            persistenceMock.Setup(p => p.TotalResults(defaultOptions))
                .Returns(20);

            Search<Product> target = new Search<Product>(persistenceMock.Object, defaultOptions);
            
            target.GoToPage(pageNumber);
        }

        private static List<Product> GeneratePage(ModelSearchOptions options, long length)
        {
            List<Product> products = new List<Product>();

            for (long i = 0; i < length; ++i)
            {
                products.Add(new Product()
                {
                    Id = i,
                    Name = string.Format("Product {0}", i),
                    Quantity = 1,
                    ListType = GenerateListType(options),
                    Category = GenerateCategory(options)
                });
            }

            return products;
        }

        private static ItemCategory GenerateCategory(ModelSearchOptions options)
        {
            if (options.ItemCategory != null)
            {
                return options.ItemCategory;
            }
            else
            {
                return new ItemCategory()
                {
                    Id = 1,
                    CategoryType = ItemCategoryType.Product,
                    Name = "Test Category"
                };
            }
        }

        private static ItemListType GenerateListType(ModelSearchOptions options)
        {
            if (options.AllListTypes)
            {
                return ItemListType.Have;
            }
            else
            {
                return options.ListType;
            }
        }
    }
}
