// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public interface ISearchablePersistence<T>
    {
        List<T> Page(ModelSearchOptions options, long pageNumber);

        long TotalResults(ModelSearchOptions options);
    }
}
