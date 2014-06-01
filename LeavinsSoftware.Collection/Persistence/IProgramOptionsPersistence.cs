// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Interface for <see cref="ProgramOptions"/> persistence.
    /// </summary>
    public interface IProgramOptionsPersistence
    {
        /// <summary>
        /// Saves the <see cref="ProgramOptions"/> instance, overriding
        /// current options if they already exist.
        /// </summary>
        /// <param name="options"></param>
        void Update(ProgramOptions options);

        /// <summary>
        /// Retrieves <see cref="ProgramOptions"/> currently in persistence.
        /// </summary>
        /// <remarks>
        /// If there is no <see cref="ProgramOptions"/> currently being persisted,
        /// implementors need to return a pre-initialized instance.
        /// </remarks>
        /// <returns></returns>
        ProgramOptions Retrieve();
    }
}
