// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Interface for <see cref="ComicBookSeries"/> persistence.
    /// </summary>
    /// <remarks>
    /// Classes implementing this interface persist <see cref="ComicBookSeries"/>
    /// instances but enable users to search <see cref="ComicBookSummary"/>
    /// instances. This is because <see cref="ComicBookSeries"/> instances have one
    /// or more <see cref="ComicBookSeriesEntry"/> instances, but
    /// <see cref="ComicBookSummary"/> only contains a count of issues.
    /// 
    /// This allows for better search performance because implementing
    /// classes don't need to grab every individual issue for every search.
    /// </remarks>
    public interface IComicBookPersistence :
        IPersistence<ComicBookSeries>, ISearchablePersistence<ComicBookSeries>
    {
    }
}
