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
    /// ViewModel for <see cref="ImportPage"/>.
    /// </summary>
    public sealed class ImportViewModel : ViewModelBase
    {
        public ImportViewModel(IAppNavigationService nav, IFilePicker filePicker)
        {
            mergeImportData = true;

            Nav = nav;
            FilePicker = filePicker;

            Import = new CustomCommand(
                (x) =>
                {
                    string importFileName = FilePicker.OpenFile(new string[] { "xml" });

                    if (string.IsNullOrEmpty(importFileName))
                    {
                        return;
                    }
                    
                    (new PersistenceImporter(Persistence.Container))
                        .Import(importFileName, new ImportOptions(merge: MergeImportData));

                    // TODO: Catch possible exceptions

                    BasicMessenger.Default.Send(MessageIds.App_ImportSuccess);
                    Nav.GoBack();
                });
        }

        public ICommand Import { get; private set; }

        public IAppNavigationService Nav { get; private set; }

        public IFilePicker FilePicker { get; private set; }

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

        private bool mergeImportData;
    }
}
