// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    public sealed class ExportData
    {
        public ExportData()
        {
            ComicBooks = new List<ComicBook>();
            Products = new List<Product>();
            VideoGames = new List<VideoGame>();
        }
        public List<ComicBook> ComicBooks { get; set; }

        public List<Product> Products { get; set; }

        public List<VideoGame> VideoGames { get; set; }
    }
}
