// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    /// <summary>
    /// Metadata for <see cref="IDataImportFormat"/> and
    /// <see cref="IDataExportFormat"/> implementations
    /// </summary>
    public interface IDataFormatMetadata
    {
        /// <summary>
        /// Semicolon-separated list of extensions handled by the format.
        /// </summary>
        string Extensions { get; }
    }
}
