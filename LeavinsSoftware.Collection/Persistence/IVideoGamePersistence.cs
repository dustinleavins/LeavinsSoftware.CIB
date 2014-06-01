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
    /// Interface for <see cref="VideoGame"/> persistence.
    /// </summary>
    public interface IVideoGamePersistence : IPersistence<VideoGame>, ISearchablePersistence<VideoGame>
    {
    }
}
