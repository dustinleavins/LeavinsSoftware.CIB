// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence.Export.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    public sealed class PersistenceImporter
    {
        private PersistenceImporter()
        {
        }

        public IComicBookPersistence ComicBookPersistence { get; private set; }

        public IProductPersistence ProductPersistence { get; private set; }

        public IVideoGamePersistence VideoGamePersistence { get; private set; }

        public ICategoryPersistence CategoryPersistence { get; private set; }

        public static ImporterBuilder New()
        {
            return new ImporterBuilder();
        }

        public void Import(string filename, ImportOptions importOptions)
        {
            DataFormats formats = new DataFormats("."); // Current directory
            IDataImportFormat dataFormat = formats
                .GetImportInstanceForExtension((new FileInfo(filename)).Extension);

            ExportData importData = dataFormat.Import(filename);

            ImportData(importData.ComicBooks, importOptions);
            ImportData(importData.Products, importOptions);
            ImportData(importData.VideoGames, importOptions);
        }

        private void ImportData(IEnumerable<ComicBook> list, ImportOptions importOptions)
        {
            // Get all categories & add them
            HashSet<ItemCategory> categories = new HashSet<ItemCategory>();

            foreach (ComicBook book in list)
            {
                categories.Add(book.Publisher);
            }

            PersistCategories(categories, ItemCategoryType.ComicBook, importOptions);

            // TODO: Throw exception if issue is not present in a book

            foreach (ComicBook book in list)
            {
                // Pair each comic book with its persisted publisher
                ItemCategory matchingCategory = categories
                    .Single(c => IsCategoryMatch(c, book.Publisher));

                book.Publisher = matchingCategory;

                if (importOptions.Merge)
                {
                    ComicBook match = FindMatch(book);

                    if (match != null)
                    {
                        ComicBookPersistence.Update(Merge(match, book));
                    }
                    else
                    {
                        // Book is new
                        book.Id = 0;

                        foreach (ComicBookIssue issue in book.Issues)
                        {
                            issue.Id = 0;
                            issue.ComicBookId = 0;
                        }

                        ComicBookPersistence.Create(book);
                    }
                }
                else
                {
                    // Book is new
                    book.Id = 0;

                    foreach (ComicBookIssue issue in book.Issues)
                    {
                        issue.Id = 0;
                        issue.ComicBookId = 0;
                    }

                    ComicBookPersistence.Create(book);
                }
            }
        }

        private void ImportData(IEnumerable<Product> list, ImportOptions importOptions)
        {
            // Get all categories & add them
            HashSet<ItemCategory> categories = new HashSet<ItemCategory>();

            foreach (Product importProduct in list)
            {
                categories.Add(importProduct.Category);
            }

            PersistCategories(categories, ItemCategoryType.Product, importOptions);

            foreach (Product importProduct in list)
            {
                // Pair each product with its persisted category
                ItemCategory matchingCategory = categories
                    .First(c => IsCategoryMatch(c, importProduct.Category));

                importProduct.Category = matchingCategory;

                if (importOptions.Merge)
                {
                    Product existingProduct = FindMatch(importProduct);

                    if (existingProduct != null)
                    {
                        ProductPersistence.Update(Merge(existingProduct, importProduct));
                    }
                    else
                    {
                        // New product
                        importProduct.Id = 0;

                        ProductPersistence.Create(importProduct);
                    }
                }
                else
                {
                    importProduct.Id = 0;

                    ProductPersistence.Create(importProduct);
                }
            }
        }

        private void ImportData(IEnumerable<VideoGame> list, ImportOptions importOptions)
        {
            // Get all categories & add them
            HashSet<ItemCategory> categories = new HashSet<ItemCategory>();

            foreach (VideoGame game in list)
            {
                categories.Add(game.System);
            }

            PersistCategories(categories, ItemCategoryType.VideoGame, importOptions);

            foreach (VideoGame gameFromImportFile in list)
            {
                // Pair each comic book with its persisted publisher & add it
                ItemCategory matchingCategory = categories
                    .First(c => IsCategoryMatch(c, gameFromImportFile.System));

                gameFromImportFile.System = matchingCategory;

                if (importOptions.Merge)
                {
                    VideoGame existingProduct = FindMatch(gameFromImportFile);

                    if (existingProduct != null)
                    {
                        VideoGamePersistence.Update(Merge(existingProduct, gameFromImportFile));
                    }
                    else
                    {
                        // New game
                        gameFromImportFile.Id = 0;

                        VideoGamePersistence.Create(gameFromImportFile);
                    }
                }
                else
                {
                    gameFromImportFile.Id = 0;

                    VideoGamePersistence.Create(gameFromImportFile);
                }
            }
        }

        /// <summary>
        /// Persists and updates categories from the import file.
        /// </summary>
        /// <param name="newCategories"></param>
        /// <param name="categoryType"></param>
        /// <param name="importOptions"></param>
        private void PersistCategories(HashSet<ItemCategory> newCategories,
            ItemCategoryType categoryType,
            ImportOptions importOptions)
        {
            if (importOptions.Merge)
            {
                ICollection<ItemCategory> existingCategories =
                    CategoryPersistence.RetrieveAll(categoryType);

                ICollection<ItemCategory> toReplaceInNewCategories = new List<ItemCategory>();

                foreach (ItemCategory categoryToAdd in newCategories)
                {
                    bool matchFound = existingCategories
                        .Any((existing) => IsCategoryMatch(categoryToAdd, existing));

                    if (matchFound)
                    {
                        toReplaceInNewCategories.Add(categoryToAdd);
                    }
                    else
                    {
                        // Add Category
                        categoryToAdd.Id = 0;
                        CategoryPersistence.Create(categoryToAdd);
                    }
                }

                foreach (ItemCategory categoryToReplace in toReplaceInNewCategories)
                {
                    // Identify match
                    ItemCategory matchingCategory = existingCategories
                        .First((existing) => IsCategoryMatch(categoryToReplace, existing));

                    // Replace in newCategories set
                    newCategories.Remove(categoryToReplace);
                    newCategories.Add(matchingCategory);
                }
            }
            else
            {
                foreach (ItemCategory categoryToAdd in newCategories)
                {
                    categoryToAdd.Id = 0;
                    CategoryPersistence.Create(categoryToAdd);
                }
            }
        }

        private static bool IsCategoryMatch(ItemCategory categoryL, ItemCategory categoryR)
        {
            if (string.IsNullOrWhiteSpace(categoryL.Code) &&
                string.IsNullOrWhiteSpace(categoryR.Code))
            {
                // Compare names
               return string.Equals(categoryL.Name,
                   categoryR.Name,
                   StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return string.Equals(categoryL.Code,
                    categoryR.Code,
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        private ComicBook FindMatch(ComicBook bookFromImportFile)
        {
            ComicBookSummary matchingSummary = null;
            ComicBook match = null;

            foreach (var page in ComicBookPersistence.AllPages())
            {
                // Search for match in page
                matchingSummary = page.FirstOrDefault(existing =>
                    {
                        return string.Equals(bookFromImportFile.Name,
                            existing.Name,
                            StringComparison.OrdinalIgnoreCase) &&

                            IsCategoryMatch(bookFromImportFile.Publisher, existing.Publisher);
                    });

                if (matchingSummary != null)
                {
                    // found match
                    break;
                }
            }

            if (matchingSummary != null)
            {
                match = ComicBookPersistence.Retrieve(matchingSummary.Id);
            }

            return match;
        }

        private ComicBook Merge(ComicBook existingBook, ComicBook bookFromImportFile)
        {
            ComicBook mergedBook = new ComicBook();
            mergedBook.Name = existingBook.Name;
            mergedBook.Id = existingBook.Id;
            mergedBook.Publisher = existingBook.Publisher;
            mergedBook.Notes = existingBook.Notes + bookFromImportFile.Notes;

            // Merge issues
            foreach (ComicBookIssue issueFromImportFile in bookFromImportFile.Issues)
            {
                // Find matching, existing one
                ComicBookIssue existingMatch = existingBook.Issues.FirstOrDefault((i) =>
                    {
                        // Does not compare ListType and Notes
                        bool issueNumberMatch = string.Equals(i.IssueNumber,
                            issueFromImportFile.IssueNumber,
                            StringComparison.OrdinalIgnoreCase);

                        bool issueTypeMatch = i.IssueType == issueFromImportFile.IssueType;

                        bool nameMatch = string.Equals(i.Name, issueFromImportFile.Name,
                            StringComparison.OrdinalIgnoreCase);

                        bool coverMatch = string.Equals(i.Cover, issueFromImportFile.Cover,
                            StringComparison.OrdinalIgnoreCase);

                        bool conditionMatch = string.Equals(i.Condition,
                            issueFromImportFile.Condition,
                            StringComparison.OrdinalIgnoreCase);

                        bool distMatch = i.DistributionType == issueFromImportFile.DistributionType;

                        return issueNumberMatch &&
                            issueTypeMatch &&
                            nameMatch &&
                            coverMatch &&
                            conditionMatch &&
                            distMatch;
                    });

                if (existingMatch == null)
                {
                    // Add issue to existing book
                    issueFromImportFile.Id = 0;
                    issueFromImportFile.ComicBookId = 0;
                    mergedBook.Issues.Add(issueFromImportFile);
                }
                else
                {
                    // Update existing issue
                    existingMatch.ListType = issueFromImportFile.ListType;
                    existingMatch.Notes = existingMatch.Notes + issueFromImportFile.Notes;
                    mergedBook.Issues.Add(existingMatch);
                }
            }

            return mergedBook;
        }

        private Product FindMatch(Product productFromImportFile)
        {
            Product match = null;

            foreach (var page in ProductPersistence.AllPages())
            {
                // Search for match in page
                match = page.FirstOrDefault(existing =>
                {
                    bool titleMatch = string.Equals(productFromImportFile.Name,
                        existing.Name,
                        StringComparison.OrdinalIgnoreCase);

                    bool categoryMatch = IsCategoryMatch(productFromImportFile.Category,
                        existing.Category);

                    return titleMatch && categoryMatch;
                });

                if (match != null)
                {
                    // found match
                    break;
                }
            }

            return match;
        }

        private Product Merge(Product existingProduct, Product productFromImportFile)
        {
            Product mergedProduct = new Product();
            mergedProduct.Category = existingProduct.Category;
            mergedProduct.Id = existingProduct.Id;
            mergedProduct.ListType = productFromImportFile.ListType;
            mergedProduct.Name = existingProduct.Name;
            mergedProduct.Quantity = productFromImportFile.Quantity;
            mergedProduct.Notes = existingProduct.Notes + productFromImportFile.Notes;

            return mergedProduct;
        }

        private VideoGame FindMatch(VideoGame gameFromImportFile)
        {
            VideoGame match = null;

            foreach (var page in VideoGamePersistence.AllPages())
            {
                // Search for match in page
                match = page.FirstOrDefault(existing =>
                {
                    bool titleMatch = string.Equals(gameFromImportFile.Name,
                        existing.Name,
                        StringComparison.OrdinalIgnoreCase);

                    bool categoryMatch = IsCategoryMatch(gameFromImportFile.System,
                        existing.System);

                    bool conditionMatch = string.Equals(gameFromImportFile.Condition,
                        existing.Condition,
                        StringComparison.OrdinalIgnoreCase);

                    bool providerMatch = string.Equals(gameFromImportFile.ContentProvider,
                        existing.ContentProvider,
                        StringComparison.OrdinalIgnoreCase);

                    bool distributionMatch = gameFromImportFile.DistributionType == existing.DistributionType;

                    return titleMatch && 
                        categoryMatch &&
                        conditionMatch &&
                        providerMatch &&
                        distributionMatch;
                });

                if (match != null)
                {
                    // found match
                    break;
                }
            }

            return match;
        }

        private VideoGame Merge(VideoGame existing, VideoGame gameFromImportFile)
        {
            VideoGame merged = new VideoGame();

            merged.Id = existing.Id;
            merged.Name = existing.Name;
            merged.Condition = existing.Condition;
            merged.ContentProvider = existing.ContentProvider;
            merged.DistributionType = existing.DistributionType;
            merged.ListType = gameFromImportFile.ListType;
            merged.Notes = existing.Notes + gameFromImportFile.Notes;
            merged.System = existing.System;

            return merged;
        }

        public class ImporterBuilder
        {
            public ImporterBuilder()
            {
            }

            public ImporterBuilder ComicBookPersistence(IComicBookPersistence instance)
            {
                comicBookPersistence = instance;
                return this;
            }

            public ImporterBuilder ProductPersistence(IProductPersistence instance)
            {
                productPersistence = instance;
                return this;
            }

            public ImporterBuilder VideoGamePersistence(IVideoGamePersistence instance)
            {
                videoGamePersistence = instance;
                return this;
            }

            public ImporterBuilder CategoryPersistence(ICategoryPersistence instance)
            {
                categoryPersistence = instance;
                return this;
            }

            public PersistenceImporter Build()
            {
                return new PersistenceImporter()
                {
                    ComicBookPersistence = comicBookPersistence,
                    ProductPersistence = productPersistence,
                    VideoGamePersistence = videoGamePersistence,
                    CategoryPersistence = categoryPersistence
                };
            }

            private IComicBookPersistence comicBookPersistence;

            private IProductPersistence productPersistence;

            private IVideoGamePersistence videoGamePersistence;

            private ICategoryPersistence categoryPersistence;
        }
    }
}
