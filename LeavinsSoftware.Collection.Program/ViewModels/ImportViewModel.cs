// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Testing;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Persistence.Export;
using LeavinsSoftware.Collection.Program.Resources;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class ImportViewModel : ViewModelBase
    {
        public ImportViewModel(IAppNavigationService nav)
        {
            mergeImportData = true;

            Nav = nav;

            Import = new CustomCommand(
                (x) =>
                {
                    if (DoImport())
                    {
                        MessageBox.Show(InterfaceResources.Import_SuccessMessage);
                        Nav.GoBack();
                    }
                });
        }

        public ICommand Import { get; private set; }

        public IAppNavigationService Nav { get; private set; }

        public bool MergeImportData
        {
            get
            {
                return mergeImportData;
            }
            set
            {
                if (mergeImportData != value)
                {
                    mergeImportData = value;
                    OnPropertyChanged("MergeImportData");
                }
            }
        }

        private bool DoImport()
        {
            string sourceFileName = string.Empty;

            var dialog = new OpenFileDialog();
            dialog.Filter = "xml files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                sourceFileName = dialog.FileName;
            }

            if (string.IsNullOrEmpty(sourceFileName))
            {
                return false;
            }

            // Create exporter instance & export
            PersistenceImporter.New()
                .ComicBookPersistence(Persistence.ComicPersistence)
                .ProductPersistence(Persistence.ProductPersistence)
                .VideoGamePersistence(Persistence.GamePersistence)
                .CategoryPersistence(Persistence.CategoryPersistence)
                .Build()
                .Import(sourceFileName, new ImportOptions(merge: MergeImportData));

            // TODO: Catch possible exceptions

            return true;
        }

        private bool mergeImportData;
    }
}
