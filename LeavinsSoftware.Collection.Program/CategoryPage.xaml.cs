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

        private CategoryPage()
        {
            InitializeComponent();
        }

        [Obsolete("Use generic version of this method")]
        public static CategoryPage PageFor(ItemCategoryType type)
        {
            var page = new CategoryPage();
            switch(type)
            {
                case ItemCategoryType.ComicBook:
                    page.model = CategoryViewModel.Create<ComicBookSeries>(new PageNavigationService(page));
                    break;
                case ItemCategoryType.Product:
                    page.model = CategoryViewModel.Create<Product>(new PageNavigationService(page));
                    break;
                case ItemCategoryType.VideoGame:
                    page.model = CategoryViewModel.Create<VideoGame>(new PageNavigationService(page));
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            page.DataContext = page.model;

            // Set title; if it just used binding, it appears as blank
            // when the user first navigates to a CategoryPage
            page.Title = page.model.Title;
            
            return page;
        }
        
        public static CategoryPage PageFor<TItem>() where TItem : Item
        {
            var page = new CategoryPage();
            page.model = CategoryViewModel.Create<TItem>(new PageNavigationService(page));
            page.DataContext = page.model;
            
            // Set title; if it just used binding, it appears as blank
            // when the user first navigates to a CategoryPage
            page.Title = page.model.Title;
            
            return page;
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