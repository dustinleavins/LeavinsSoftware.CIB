// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class Search<T>
    {
        public Search(ISearchablePersistence<T> persistence, ModelSearchOptions options)
        {
            Persistence = persistence;
            Options = options;
            GoToPage(0);
        }

        public List<T> CurrentPage { get; private set; }

        public long CurrentPageNumber { get; private set; }

        public long TotalNumberOfPages { get; private set; }

        public void NextPage()
        {
            if (HasNextPage)
            {
                GoToPage(CurrentPageNumber + 1);
            }
        }

        public void PreviousPage()
        {
            if (HasPreviousPage)
            {
                GoToPage(CurrentPageNumber - 1);
            }
        }

        public void GoToPage(long newPageNumber)
        {
            CurrentPageNumber = newPageNumber;
            CurrentPage = Persistence.Page(Options, newPageNumber);
        }

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

        public bool HasPreviousPage
        {
            get { return CurrentPageNumber != 0; }
        }

        public ISearchablePersistence<T> Persistence { get; private set; }

        public ModelSearchOptions Options { get; private set; }
    }
}
