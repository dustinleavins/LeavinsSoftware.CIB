// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export.Extensions
{
    /// <summary>
    /// Extension methods used during import and export.
    /// </summary>
    public static class ExportExtensions
    {
        /// <summary>
        /// Retrieves an enumerator with pages of results; all pages combine
        /// have all items of type T that are currently being persistend.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="persistence"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> AllPages<T>(
            this ISearchablePersistence<T> persistence)
        {
            ModelSearchOptions options = new ModelSearchOptionsBuilder()
            {
                ItemsPerPage = 100
            }.Build();

            return AllPages(persistence, options);
        }

        /// <summary>
        /// Returns an enumerator with pages of results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="persistence"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
