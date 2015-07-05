// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for OptionsCategoryNamesPage.xaml
    /// </summary>
    public partial class OptionsCategoryNamesPage : Page
    {
        private readonly OptionsCategoryNamesViewModel model;
        public OptionsCategoryNamesPage()
        {
            InitializeComponent();
            model = new OptionsCategoryNamesViewModel(new PageNavigationService(this));
            DataContext = model;
        }
    }
}
