// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomeViewModel(new PageNavigationService(this));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 'Add Category'
            if (Persistence.GetInstance<ICategoryPersistence>().Any())
            {
                welcomeMessageGrid.Visibility = System.Windows.Visibility.Collapsed;
                importExportGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                welcomeMessageGrid.Visibility = System.Windows.Visibility.Visible;
                importExportGrid.Visibility = System.Windows.Visibility.Collapsed;

                BasicMessenger.Default.Send(MessageIds.App_Welcome);
            }
        }
    }
}
