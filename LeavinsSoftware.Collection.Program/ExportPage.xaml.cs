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
    public partial class ExportPage : Page
    {
        private ExportViewModel viewModel;
        public ExportPage()
        {
            InitializeComponent();
            DataContext = viewModel = new ExportViewModel(new PageNavigationService(this));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Messenger.Register(ExportViewModel.ExportFileNameMessage,
                () =>
                {
                    var dialog = new SaveFileDialog();
                    dialog.Filter = "xml files (*.xml)|*.xml|All Files (*.*)|*.*";
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        viewModel.ExportFileName = dialog.FileName;
                    }
                });

            viewModel.Messenger.Register(ExportViewModel.FinishedExportMessage,
                () =>
                {
                    MessageBox.Show(InterfaceResources.Export_SuccessMessage);
                });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Messenger.Unregister(ExportViewModel.ExportFileNameMessage);
            viewModel.Messenger.Unregister(ExportViewModel.FinishedExportMessage);
        }
    }
}