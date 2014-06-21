﻿// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
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
        /// <summary>
        /// ID associated with the 'ImportFileName' message.
        /// </summary>
        public const string ImportFileNameMessage = "ImportFileName";

        /// <summary>
        /// ID associated with the 'ImportFileName' message.
        /// </summary>
        public const string FinishedImportMessage = "FinishedImport";

        public ImportViewModel(IAppNavigationService nav)
        {
            Messenger = new BasicMessenger();
            mergeImportData = true;

            Nav = nav;

            Import = new CustomCommand(
                (x) =>
                {
                    Messenger.Send(ImportFileNameMessage);

                    if (string.IsNullOrEmpty(ImportFileName))
                    {
                        return;
                    }
                    
                    (new PersistenceImporter(Persistence.Container))
                        .Import(ImportFileName, new ImportOptions(merge: MergeImportData));

                    // TODO: Catch possible exceptions

                    Messenger.Send(FinishedImportMessage);
                    Nav.GoBack();
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

        public BasicMessenger Messenger { get; private set; }

        public string ImportFileName
        {
            get
            {
                return importFileName;
            }
            set
            {
                if (!string.Equals(importFileName, value, StringComparison.Ordinal))
                {
                    importFileName = value;
                    OnPropertyChanged("ImportFileName");
                }
            }
        }

        private bool mergeImportData;
        private string importFileName;
    }
}
