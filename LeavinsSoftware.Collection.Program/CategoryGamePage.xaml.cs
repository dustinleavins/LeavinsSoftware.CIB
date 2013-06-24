﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for CategoryGamePage.xaml
    /// </summary>
    public partial class CategoryGamePage : Page
    {
        private CategoryGameViewModel model;

        public CategoryGamePage()
        {
            InitializeComponent();
            model = new CategoryGameViewModel(new PageNavigationService(this));
            DataContext = model;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            model.LoadedHandler();
            linksPanel.Children.Clear();

            foreach (ItemCategory category in model.Categories)
            {
                Hyperlink categoryLink = new Hyperlink(new Run(category.Name));
                categoryLink.Command = model.GoTo;
                categoryLink.CommandParameter = category;
                Label categoryLabel = new Label();
                categoryLabel.Content = categoryLink;
                categoryLabel.Style = (Style)Application.Current.FindResource("categoryLabelStyle");

                linksPanel.Children.Add(categoryLabel);
            }
        }
    }
}
