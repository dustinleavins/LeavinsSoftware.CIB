// Copyright (c) 2013 Dustin Leavins
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
    /// Interaction logic for CategoryProductPage.xaml
    /// </summary>
    public partial class CategoryProductPage : CategoryPage
    {
        private CategoryProductViewModel model;

        public CategoryProductPage()
        {
            InitializeComponent();
            model = new CategoryProductViewModel(new PageNavigationService(this));
            DataContext = model;

            foreach (ItemCategory category in model.Categories)
            {
                Hyperlink categoryLink = new Hyperlink(new Run(category.Name));
                categoryLink.Command = model.GoTo;
                categoryLink.CommandParameter = category;
                Label categoryLabel = new Label();
                categoryLabel.Style = (Style)Application.Current.FindResource("categoryLabelStyle");

                linksPanel.Children.Add(categoryLabel);
            }
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
