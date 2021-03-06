﻿// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleInjector;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence.Export.Extensions;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    /// <summary>
    /// Encapsulates the process of importing program data from a file.
    /// </summary>
    public sealed class PersistenceImporter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        /// <exception cref="T:System.ArgumentException">
        /// Thrown when container does not specify a necessary instance.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when container is null.
        /// </exception>
        public PersistenceImporter(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            
            try
            {
                ComicBookPersistence = container.GetInstance<ISearchablePersistence<ComicBookSeries>>();
                ProductPersistence = container.GetInstance<ISearchablePersistence<Product>>();
                VideoGamePersistence = container.GetInstance<ISearchablePersistence<VideoGame>>();
                CategoryPersistence = container.GetInstance<ICategoryPersistence>();
            }
            catch (ActivationException e)
            {
                throw new ArgumentException("container does not specify necessary instances",
                    "container",
                    e);
            }
        }

        public ISearchablePersistence<ComicBookSeries> ComicBookPersistence { get; private set; }

        public ISearchablePersistence<Product> ProductPersistence { get; private set; }

        public ISearchablePersistence<VideoGame> VideoGamePersistence { get; private set; }

        public ICategoryPersistence CategoryPersistence { get; private set; }
        
        public void Import(string fileName, ImportOptions importOptions)
        {
            IDataImportFormat dataFormat = null;
            using (DataFormats formats = new DataFormats(".")) // Current directory
            {
                dataFormat = formats
                    .GetImportInstanceForExtension((new FileInfo(fileName)).Extension);
            }

            ExportData importData = dataFormat.Import(fileName);

            ImportData(importData.ComicBooks, importOptions);
            ImportData(importData.Products, importOptions);
            ImportData(importData.VideoGames, importOptions);
        }

        private void ImportData(IEnumerable<ComicBookSeries> list, ImportOptions importOptions)
        {
            // Get all categories & add them
            HashSet<ItemCategory> categories = new HashSet<ItemCategory>();

            foreach (ComicBookSeries book in list)
            {
                categories.Add(book.Publisher);
            }

            PersistCategories(categories, ItemCategoryType.ComicBook, importOptions);

            foreach (ComicBookSeries book in list)
            {

                if (!book.Entries.Any())
                {
                    throw new InvalidDataException(
                        "Tried to import a comic book series without an entry.");
                }

                // Pair each comic book with its persisted publisher
                ItemCategory matchingCategory = categories
                    .Single(c => IsCategoryMatch(c, book.Publisher));

                book.Publisher = matchingCategory;

                if (importOptions.Merge)
                {
                    ComicBookSeries match = FindMatch(book);

                    if (match != null)
                    {
                        ComicBookPersistence.Update(Merge(match, book));
                    }
                    else
                    {
                        // Book is new
                        book.Id = 0;

                        foreach (ComicBookSeriesEntry issue in book.Entries)
                        {
                            issue.Id = 0;
                            issue.SeriesId = 0;
                        }

                        ComicBookPersistence.Create(book);
                    }
                }
                else
                {
                    // Book is new
                    book.Id = 0;

                    foreach (ComicBookSeriesEntry issue in book.Entries)
                    {
                        issue.Id = 0;
                        issue.SeriesId = 0;
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

        private ComicBookSeries FindMatch(ComicBookSeries bookFromImportFile)
        {
            ComicBookSeries matchingSummary = null;
            ComicBookSeries match = null;

            foreach (var page in ComicBookPersistence.AllPages())
            {
                // Search for match in page
                matchingSummary = page.FirstOrDefault(existing =>
                    string.Equals(bookFromImportFile.Name, existing.Name, StringComparison.OrdinalIgnoreCase) &&
                    IsCategoryMatch(bookFromImportFile.Publisher, existing.Publisher));

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

        private static ComicBookSeries Merge(ComicBookSeries existingBook, ComicBookSeries bookFromImportFile)
        {
            ComicBookSeries mergedBook = new ComicBookSeries();
            mergedBook.Name = existingBook.Name;
            mergedBook.Id = existingBook.Id;
            mergedBook.Publisher = existingBook.Publisher;
            mergedBook.Notes = MergeNotes(existingBook.Notes, bookFromImportFile.Notes);

            // Merge issues
            foreach (ComicBookSeriesEntry issueFromImportFile in bookFromImportFile.Entries)
            {
                // Find matching, existing one
                ComicBookSeriesEntry existingMatch = existingBook.Entries.FirstOrDefault((i) =>
                    {
                        // Does not compare ListType and Notes
                        bool issueNumberMatch = string.Equals(i.Number,
                            issueFromImportFile.Number,
                            StringComparison.OrdinalIgnoreCase);

                        bool issueTypeMatch = i.EntryType == issueFromImportFile.EntryType;

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
                    issueFromImportFile.SeriesId = 0;
                    mergedBook.Entries.Add(issueFromImportFile);
                }
                else
                {
                    // Update existing issue
                    existingMatch.ListType = issueFromImportFile.ListType;
                    existingMatch.Notes = MergeNotes(existingMatch.Notes, issueFromImportFile.Notes);
                    mergedBook.Entries.Add(existingMatch);
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

        private static Product Merge(Product existingProduct, Product productFromImportFile)
        {
            Product mergedProduct = new Product();
            mergedProduct.Category = existingProduct.Category;
            mergedProduct.Id = existingProduct.Id;
            mergedProduct.ListType = productFromImportFile.ListType;
            mergedProduct.Name = existingProduct.Name;
            mergedProduct.Quantity = productFromImportFile.Quantity;
            mergedProduct.Notes = MergeNotes(existingProduct.Notes, productFromImportFile.Notes);

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

        private static VideoGame Merge(VideoGame existing, VideoGame gameFromImportFile)
        {
            VideoGame merged = new VideoGame();

            merged.Id = existing.Id;
            merged.Name = existing.Name;
            merged.Condition = existing.Condition;
            merged.ContentProvider = existing.ContentProvider;
            merged.DistributionType = existing.DistributionType;
            merged.ListType = gameFromImportFile.ListType;
            merged.Notes = MergeNotes(existing.Notes, gameFromImportFile.Notes);
            merged.System = existing.System;

            return merged;
        }
        
        private static string MergeNotes(string existingNotes, string actualNotes)
        {
            if (string.Equals(existingNotes, actualNotes, StringComparison.Ordinal))
            {
                return existingNotes;
            }
            else
            {
                return existingNotes + "\n" + actualNotes;
            }
        }
    }
}
