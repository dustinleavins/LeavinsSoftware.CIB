// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    public partial class ExportPage : Page
    {
        public ExportPage()
        {
            InitializeComponent();
            DataContext = new ExportViewModel(new PageNavigationService(this));
        }
    }
}