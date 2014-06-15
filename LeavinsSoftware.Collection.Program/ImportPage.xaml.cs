// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for ImportPage.xaml
    /// </summary>
    public partial class ImportPage : Page
    {
        private readonly ImportViewModel viewModel;
        public ImportPage()
        {
            InitializeComponent();
            DataContext = viewModel = new ImportViewModel(new PageNavigationService(this));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Messenger.Register(ImportViewModel.ImportFileNameMessage,
                () =>
                {
                    var dialog = new OpenFileDialog();
                    dialog.Filter = "xml files (*.xml)|*.xml|All Files (*.*)|*.*";
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        viewModel.ImportFileName = dialog.FileName;
                    }
                });

            viewModel.Messenger.Register(ImportViewModel.FinishedImportMessage,
                () => MessageBox.Show(InterfaceResources.Import_SuccessMessage));
        }

        private void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Messenger.Unregister(ImportViewModel.ImportFileNameMessage);
            viewModel.Messenger.Unregister(ImportViewModel.FinishedImportMessage);
        }
    }
}