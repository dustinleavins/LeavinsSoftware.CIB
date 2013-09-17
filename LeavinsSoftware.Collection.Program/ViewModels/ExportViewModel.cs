// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Persistence.Export;
using System;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="ExportPage"/>.
    /// </summary>
    public sealed class ExportViewModel : ViewModelBase
    {
        /// <summary>
        /// ID associated with the 'ExportFileName' message.
        /// </summary>
        public const string ExportFileNameMessage = "ExportFileName";

        /// <summary>
        /// ID associated with the 'FinishedExport' message.
        /// </summary>
        public const string FinishedExportMessage = "FinishedExport";

        public ExportViewModel(IAppNavigationService nav)
        {
            Messenger = new BasicMessenger();
            Nav = nav;

            Export = new CustomCommand(
                (x) =>
                {
                    Messenger.Send(ExportFileNameMessage);
                    if (string.IsNullOrEmpty(ExportFileName))
                    {
                        return;
                    }

                    // Create exporter instance & export
                    PersistenceExporter.New()
                        .ComicBookPersistence(Persistence.ComicPersistence)
                        .ProductPersistence(Persistence.ProductPersistence)
                        .VideoGamePersistence(Persistence.GamePersistence)
                        .Build()
                        .Export(ExportFileName);

                    // TODO: Catch possible exceptions

                    Messenger.Send(FinishedExportMessage);
                    Nav.GoBack();
                });
        }

        public ICommand Export { get; private set; }

        public IAppNavigationService Nav { get; private set; }

        public BasicMessenger Messenger { get; private set; }

        public string ExportFileName
        {
            get
            {
                return exportFileName;
            }
            set
            {
                if (!string.Equals(exportFileName, value, StringComparison.Ordinal))
                {
                    exportFileName = value;
                    OnPropertyChanged("ExportFileName");
                }
            }
        }

        private string exportFileName;
    }
}
