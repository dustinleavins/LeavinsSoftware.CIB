// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LeavinsSoftware.Collection.Persistence.Export.Formats
{
    /// <summary>
    /// XML export format.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IDataExportFormat))]
    [ExportMetadata("Extensions", "xml")]
    public sealed class XmlExportFormat : IDataExportFormat
    {
        public XmlExportFormat()
        {
        }

        public void Export(string fileName, ExportData dataToExport)
        {
            var serializer = new XmlSerializer(typeof(ExportData));

            using (var stream = File.Create(fileName))
            {
                serializer.Serialize(stream, dataToExport);
            }
        }
    }
}
