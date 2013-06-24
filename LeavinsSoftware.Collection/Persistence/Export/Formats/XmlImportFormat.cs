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
    [System.ComponentModel.Composition.Export(typeof(IDataImportFormat))]
    [ExportMetadata("Extensions", "xml")]
    public sealed class XmlImportFormat : IDataImportFormat
    {
        public XmlImportFormat()
        {
        }

        public ExportData Import(string filename)
        {
            ExportData importData;
            var serializer = new XmlSerializer(typeof(ExportData));

            using (var stream = File.OpenRead(filename))
            {
                importData = serializer.Deserialize(stream) as ExportData;
            }

            return importData;
        }
    }
}
