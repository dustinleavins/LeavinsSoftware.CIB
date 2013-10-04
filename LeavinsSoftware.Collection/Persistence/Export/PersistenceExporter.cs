// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence.Export.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    /// <summary>
    /// Encapsulates the process of exporting program data to a file.
    /// </summary>
    public sealed class PersistenceExporter
    {
        private PersistenceExporter()
        {
        }

        public IComicBookPersistence ComicBookPersistence { get; private set; }

        public IProductPersistence ProductPersistence { get; private set; }

        public IVideoGamePersistence VideoGamePersistence { get; private set; }

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

        public static ExporterBuilder New()
        {
            return new ExporterBuilder();
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
                foreach (ComicBookSummary summary in pageOfSummaries)
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

        /// <summary>
        /// Builds an object to perform data export.
        /// </summary>
        public class ExporterBuilder
        {
            public ExporterBuilder()
            {
            }

            public ExporterBuilder ComicBookPersistence(IComicBookPersistence instance)
            {
                comicBookPersistence = instance;
                return this;
            }

            public ExporterBuilder ProductPersistence(IProductPersistence instance)
            {
                productPersistence = instance;
                return this;
            }

            public ExporterBuilder VideoGamePersistence(IVideoGamePersistence instance)
            {
                videoGamePersistence = instance;
                return this;
            }

            public PersistenceExporter Build()
            {
                return new PersistenceExporter()
                {
                    ComicBookPersistence = comicBookPersistence,
                    ProductPersistence = productPersistence,
                    VideoGamePersistence = videoGamePersistence
                };
            }

            private IComicBookPersistence comicBookPersistence;

            private IProductPersistence productPersistence;

            private IVideoGamePersistence videoGamePersistence;
        }
    }
}
