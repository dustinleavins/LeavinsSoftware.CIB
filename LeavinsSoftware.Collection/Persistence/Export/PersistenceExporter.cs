// Copyright (c) 2013, 2014 Dustin Leavins
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
    /// Encapsulates the process of exporting program data to a file.
    /// </summary>
    public sealed class PersistenceExporter
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
        public PersistenceExporter(Container container)
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

        public void Export(string destinationFileName)
        {
            ExportData exportData = new ExportData();
            exportData.ComicBooks.AddRange(ComicBooks());
            exportData.Products.AddRange(Products());
            exportData.VideoGames.AddRange(VideoGames());

            IDataExportFormat dataFormat = null;
            using (DataFormats formats = new DataFormats(".")) // Current directory
            {
                dataFormat = formats
                    .GetExportInstanceForExtension((new FileInfo(destinationFileName)).Extension);
            }

            dataFormat.Export(destinationFileName, exportData);
        }
        
        private List<ComicBookSeries> ComicBooks()
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100
            }.Build();

            List<ComicBookSeries> books = new List<ComicBookSeries>();

            foreach (var pageOfSummaries in ComicBookPersistence.AllPages(options))
            {
                foreach (ComicBookSeries summary in pageOfSummaries)
                {
                    ComicBookSeries book = ComicBookPersistence.Retrieve(summary.Id);
                    books.Add(book);
                }
            }

            return books;
        }

        private List<Product> Products()
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100
            }.Build();

            List<Product> products = new List<Product>();

            foreach (var pageOfProducts in ProductPersistence.AllPages(options))
            {
                products.AddRange(pageOfProducts);
            }

            return products;
        }

        private List<VideoGame> VideoGames()
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100
            }.Build();

            List<VideoGame> games = new List<VideoGame>();

            foreach (var pageOfGames in VideoGamePersistence.AllPages(options))
            {
                games.AddRange(pageOfGames);
            }

            return games;
        }
    }
}
