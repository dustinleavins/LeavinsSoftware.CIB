// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.ViewModels;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for CategoryPage.xaml
    /// </summary>
    public partial class CategoryPage : Page
    {
        private CategoryViewModel model;
        public CategoryPage(ItemCategoryType primaryType)
        {
            InitializeComponent();
            model = CategoryViewModel.ComicBook(new PageNavigationService(this));
            switch(primaryType)
            {
                case ItemCategoryType.ComicBook:
                    model = CategoryViewModel.ComicBook(new PageNavigationService(this));
                    break;
                case ItemCategoryType.Product:
                    model = CategoryViewModel.Product(new PageNavigationService(this));
                    break;
                case ItemCategoryType.VideoGame:
                    model = CategoryViewModel.VideoGame(new PageNavigationService(this));
                    break;
                default:
                    throw new NotImplementedException();
            }
            DataContext = model;
        }

        public static CategoryPage PageFor(ItemCategoryType type)
        {
            return new CategoryPage(type);
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            model.OnLoaded();
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