﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for AddMainCategoryPage.xaml
    /// </summary>
    public partial class AddMainCategoryPage : Page
    {
        public AddMainCategoryPage()
        {
            InitializeComponent();
            DataContext = new AddMainCategoryViewModel(new PageNavigationService(this));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Product
            if (Persistence.CategoryPersistence.Any(ItemCategoryType.Product))
            {
                productCategory.Visibility = Visibility.Collapsed;
            }
            else
            {
                productCategory.Visibility = Visibility.Visible;
            }

            // Comic Book
            if (Persistence.CategoryPersistence.Any(ItemCategoryType.ComicBook))
            {
                comicCategory.Visibility = Visibility.Collapsed;
            }
            else
            {
                comicCategory.Visibility = Visibility.Visible;
            }

            // Video Game
            if (Persistence.CategoryPersistence.Any(ItemCategoryType.VideoGame))
            {
                gameCategory.Visibility = Visibility.Collapsed;
            }
            else
            {
                gameCategory.Visibility = Visibility.Visible;
            }
        }
    }
}
