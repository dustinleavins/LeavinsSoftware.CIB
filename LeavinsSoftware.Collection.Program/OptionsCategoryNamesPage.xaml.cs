// Copyright (c) 2013, 2014 Dustin Leavins
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
        private OptionsCategoryNamesViewModel model;
        public OptionsCategoryNamesPage()
        {
            InitializeComponent();
            model = new OptionsCategoryNamesViewModel(new PageNavigationService(this));
            DataContext = model;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            model.OnLoaded();
        }

        private void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            model.OnUnloaded();
        }
    }
}
