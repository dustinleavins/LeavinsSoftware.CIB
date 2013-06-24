// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export.Extensions
{
    public static class ExportExtensions
    {
        public static IEnumerable<List<T>> AllPages<T>(
            this ISearchablePersistence<T> persistence)
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100,
                AllListTypes = true
            }.Build();

            return AllPages(persistence, options);
        }

        public static IEnumerable<List<T>> AllPages<T>(
            this ISearchablePersistence<T> persistence,
            ModelSearchOptions options)
        {
            long numberOfPages = persistence.TotalResults(options);

            for (long pageNumber = 0; pageNumber < numberOfPages; ++pageNumber)
            {
                yield return persistence.Page(options, pageNumber);
            }
        }
    }
}
