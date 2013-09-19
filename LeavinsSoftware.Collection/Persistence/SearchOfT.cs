// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Represents an ongoing search.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Search<T>
    {
        public Search(ISearchablePersistence<T> persistence, ModelSearchOptions options)
        {
            Persistence = persistence;
            Options = options;
            GoToPage(0);
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
        /// Goes to the next page (if it is available)
        /// </summary>
        /// <remarks>
        /// Currently, this just fails silently if there is no next page available.
        /// </remarks>
        public void NextPage()
        {
            if (HasNextPage)
            {
                GoToPage(CurrentPageNumber + 1);
            }
        }

        /// <summary>
        /// Goes to the previous page (unless the current page is the
        /// first one)
        /// </summary>
        /// <remarks>
        /// Currently, this just fails silently if there is no next page available.
        /// </remarks>
        public void PreviousPage()
        {
            if (HasPreviousPage)
            {
                GoToPage(CurrentPageNumber - 1);
            }
        }

        /// <summary>
        /// Goes to the specified page number (if it's available)
        /// </summary>
        /// <param name="newPageNumber"></param>
        public void GoToPage(long newPageNumber)
        {
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
