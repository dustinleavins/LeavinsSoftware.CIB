// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Persistence.Export
{
    public sealed class DataFormats : IDisposable
    {
        public DataFormats(string pluginsDirectoryName)
        {
            if (string.IsNullOrEmpty(pluginsDirectoryName))
            {
                throw new ArgumentNullException("pluginsDirectoryName",
                                                "Plugins directory cannot be null or empty");
            }

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(pluginsDirectoryName));
            container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        public Type GetImportTypeForExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return null;
            }

            string ext = string.Empty;

            if (extension.StartsWith("."))
            {
                ext = extension.Remove(0, 1);
            }
            else
            {
                ext = extension;
            }

            foreach (var item in _importFormats)
            {
                bool isRightFormat = item.Metadata
                    .Extensions
                    .ToUpperInvariant()
                    .Contains(ext.ToUpperInvariant());

                if (isRightFormat)
                {
                    return item.Value.GetType();
                }
            }

            return null;
        }

        public Type GetExportTypeForExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return null;
            }

            string ext = string.Empty;

            if (extension.StartsWith("."))
            {
                ext = extension.Remove(0, 1);
            }
            else
            {
                ext = extension;
            }

            foreach (var item in _exportFormats)
            {
                bool isRightFormat = item.Metadata
                    .Extensions
                    .ToUpperInvariant()
                    .Contains(ext.ToUpperInvariant());

                if (isRightFormat)
                {
                    return item.Value.GetType();
                }
            }

            return null;
        }

        public IDataImportFormat GetImportInstanceForExtension(string extension)
        {
            var type = GetImportTypeForExtension(extension);

            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type) as IDataImportFormat;
        }

        public IDataExportFormat GetExportInstanceForExtension(string extension)
        {
            var type = GetExportTypeForExtension(extension);

            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type) as IDataExportFormat;
        }

        public void Dispose()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<Lazy<IDataImportFormat, IDataFormatMetadata>> _importFormats;

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<Lazy<IDataExportFormat, IDataFormatMetadata>> _exportFormats;

        private CompositionContainer container;
    }
}
