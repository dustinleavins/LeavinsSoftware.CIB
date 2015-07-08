// Copyright (c) 2013-2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    public partial class ExportPage : Page
    {
        private ExportViewModel viewModel;

        public ExportPage()
        {
            InitializeComponent();
            DataContext = viewModel = new ExportViewModel(new PageNavigationService(this), new FilePicker());
        }
    }
}