// Copyright (c) 2013-2015 Dustin Leavins
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
        public ExportViewModel(IAppNavigationService nav, IFilePicker filePicker)
        {
            Nav = nav;
            FilePicker = filePicker;

            Export = new CustomCommand(
                (x) =>
                {
                    string exportFileName = FilePicker.SaveFile(new string[] { "xml" });
                    if (string.IsNullOrEmpty(exportFileName))
                    {
                        return;
                    }

                    // Create exporter instance & export
                    (new PersistenceExporter(Persistence.Container))
                        .Export(exportFileName);

                    // TODO: Catch possible exceptions

                    BasicMessenger.Default.Send(MessageIds.App_ExportSuccess);
                    Nav.GoBack();
                });
        }

        public ICommand Export { get; private set; }

        public IAppNavigationService Nav { get; private set; }

        public IFilePicker FilePicker { get; private set; }
    }
}
