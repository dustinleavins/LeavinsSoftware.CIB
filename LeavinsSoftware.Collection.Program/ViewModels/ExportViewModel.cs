// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using KSMVVM.WPF.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using LeavinsSoftware.Collection.Persistence.Export;
using LeavinsSoftware.Collection.Program.Resources;
using System.Windows;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class ExportViewModel : ViewModelBase
    {
        public ExportViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            Export = new CustomCommand(
                (x) =>
                {
                    if (DoExport())
                    {
                        MessageBox.Show(InterfaceResources.Export_SuccessMessage);
                        Nav.GoBack();
                    }
                });
        }

        private static bool DoExport()
        {
            string destinationFileName = string.Empty;

            var dialog = new SaveFileDialog();
            dialog.Filter = "xml files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                destinationFileName = dialog.FileName;
            }

            if (string.IsNullOrEmpty(destinationFileName))
            {
                return false;
            }

            // Create exporter instance & export
            PersistenceExporter.New()
                .ComicBookPersistence(Persistence.ComicPersistence)
                .ProductPersistence(Persistence.ProductPersistence)
                .VideoGamePersistence(Persistence.GamePersistence)
                .Build()
                .Export(destinationFileName);

            // TODO: Catch possible exceptions

            return true;
        }

        public ICommand Export { get; private set; }

        public IAppNavigationService Nav { get; private set; }
    }
}
