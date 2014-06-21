// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Represents an ongoing search.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Search<T> where T : Model
    {
        public Search(ISearchablePersistence<T> persistence, ModelSearchOptions options)
        {
            Persistence = persistence;
            Options = options;
            
            long totalResults = Persistence.TotalResults(options);
            
            if (totalResults == 0)
            {
                CurrentPage = new List<T>();
            }
            else
            {
                TotalNumberOfPages = totalResults / options.ItemsPerPage;

                if ((totalResults % options.ItemsPerPage) != 0)
                {
                    TotalNumberOfPages += 1;
                }
                
                GoToPage(0);
            }
        }

        /// <summary>
        /// Current page of results.
        /// </summary>
        /// <remarks>
        /// This should be the page of results currently being shown to an end-user.
        /// </remarks>
        public List<T> CurrentPage { get; private set; }

        /// <summary>
        /// Number of the current page.
        /// </summary>
        public long CurrentPageNumber { get; private set; }

        /// <summary>
        /// Total number of pages.
        /// </summary>
        public long TotalNumberOfPages { get; private set; }

        /// <summary>
        /// Goes to the next page
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This instance is currently at its last page.
        /// </exception>
        public void NextPage()
        {
            if (!HasNextPage)
            {
                throw new InvalidOperationException("Currently at last page");
            }

            GoToPage(CurrentPageNumber + 1);
        }

        /// <summary>
        /// Goes to the previous page
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This instance is currently at its first page.
        /// </exception>
        public void PreviousPage()
        {
            if (!HasPreviousPage)
            {
                throw new InvalidOperationException("Currently at first page");
            }
            
            GoToPage(CurrentPageNumber - 1);
        }

        /// <summary>
        /// Goes to the specified page number
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Specified page does not exist.
        /// </exception>
        /// <param name="newPageNumber"></param>
        public void GoToPage(long newPageNumber)
        {
            if (newPageNumber < 0 || newPageNumber >= TotalNumberOfPages)
            {
                throw new InvalidOperationException("Page does not exist");
            }
            
            CurrentPageNumber = newPageNumber;
            CurrentPage = Persistence.Page(Options, newPageNumber);
        }

        /// <summary>
        /// Is there a next page of results?
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                if (TotalNumberOfPages == 0)
                {
                    return false;
                }

                return (CurrentPageNumber + 1) != TotalNumberOfPages;
            }
        }

        /// <summary>
        /// Is there a previous page of results?
        /// </summary>
        public bool HasPreviousPage
        {
            get { return CurrentPageNumber != 0; }
        }

        /// <summary>
        /// Backing persistence class to use for actually performing searches
        /// </summary>
        public ISearchablePersistence<T> Persistence { get; private set; }

        /// <summary>
        /// Search options
        /// </summary>
        public ModelSearchOptions Options { get; private set; }
    }
}
