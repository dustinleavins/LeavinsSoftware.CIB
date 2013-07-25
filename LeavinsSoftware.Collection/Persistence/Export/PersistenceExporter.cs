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
    public sealed class PersistenceExporter
    {
        private PersistenceExporter()
        {
        }

        public IComicBookPersistence ComicBookPersistence { get; private set; }

        public IProductPersistence ProductPersistence { get; private set; }

        public IVideoGamePersistence VideoGamePersistence { get; private set; }

        public void Export(string destinationFilename)
        {
            ExportData exportData = new ExportData
            {
                ComicBooks = ComicBooks(),
                Products = Products(),
                VideoGames = VideoGames()
            };

            IDataExportFormat dataFormat = null;
            using (DataFormats formats = new DataFormats(".")) // Current directory
            {
                dataFormat = formats
                    .GetExportInstanceForExtension((new FileInfo(destinationFilename)).Extension);
            }

            dataFormat.Export(destinationFilename, exportData);
        }

        public static ExporterBuilder New()
        {
            return new ExporterBuilder();
        }

        private List<ComicBook> ComicBooks()
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100,
                AllListTypes = true
            }.Build();

            List<ComicBook> books = new List<ComicBook>();

            foreach (var pageOfSummaries in ComicBookPersistence.AllPages(options))
            {
                foreach (ComicBookSummary summary in pageOfSummaries)
                {
                    ComicBook book = ComicBookPersistence.Retrieve(summary.Id);
                    books.Add(book);
                }
            }

            return books;
        }

        private List<Product> Products()
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100,
                AllListTypes = true
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
                ItemsPerPage = 100,
                AllListTypes = true
            }.Build();

            List<VideoGame> games = new List<VideoGame>();

            foreach (var pageOfGames in VideoGamePersistence.AllPages(options))
            {
                games.AddRange(pageOfGames);
            }

            return games;
        }

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
