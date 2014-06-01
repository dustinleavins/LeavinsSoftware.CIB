// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Interface for classes that enable searches for instances of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchablePersistence<T>
    {
        /// <summary>
        /// Retrieves a page of search results.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        List<T> Page(ModelSearchOptions options, long pageNumber);

        /// <summary>
        /// Retrieves a total count of search results.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        long TotalResults(ModelSearchOptions options);
    }
}
