// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    /// <summary>
    /// Container that holds all data to be exported.
    /// </summary>
    public sealed class ExportData
    {
        public ExportData()
        {
            ComicBooks = new List<ComicBookSeries>();
            Products = new List<Product>();
            VideoGames = new List<VideoGame>();
        }
        public List<ComicBookSeries> ComicBooks { get; private set; }

        public List<Product> Products { get; private set; }

        public List<VideoGame> VideoGames { get; private set; }
    }
}
