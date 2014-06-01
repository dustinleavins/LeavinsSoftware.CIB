// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    /// <summary>
    /// Interface for classes that perform data export.
    /// </summary>
    public interface IDataExportFormat
    {
        void Export(string fileName, ExportData dataToExport);
    }
}
