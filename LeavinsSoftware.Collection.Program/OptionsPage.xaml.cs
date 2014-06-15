// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        private readonly OptionsViewModel model;

        public OptionsPage()
        {
            InitializeComponent();
            model = new OptionsViewModel(new PageNavigationService(this), () =>
                !Validation.GetHasError(proxyServerPortBox));

            DataContext = model;
        }

        private void UseProxyServer_Unchecked(object sender, RoutedEventArgs e)
        {
            // Revert invalid properties on uncheck
            if (Validation.GetHasError(proxyServerAddressBox))
            {
                model.Options.ProxyServerAddress =
                    ProgramOptions.DefaultProxyServerAddress;
            }

            if (!model.Options.IsValid("ProxyServerPort") ||
                !Regex.IsMatch(proxyServerPortBox.Text, @"^\s*\d+\s*$"))
            {
                // Clear proxyServerPortBox error
                model.Options.ProxyServerPort = 0;
                model.Options.ProxyServerPort = ProgramOptions.DefaultProxyServerPort;
            }
        }
    }
}
