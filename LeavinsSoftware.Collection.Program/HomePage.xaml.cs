// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Persistence.GetInstance<ICategoryPersistence>().Any())
            {
                initialWelcome.Visibility = Visibility.Collapsed;
                welcomeBack.Visibility = Visibility.Visible;
            }
            else
            {
                initialWelcome.Visibility = Visibility.Visible;
                welcomeBack.Visibility = Visibility.Collapsed;
                BasicMessenger.Default.Send(MessageIds.App_Welcome);
            }
        }
    }
}
