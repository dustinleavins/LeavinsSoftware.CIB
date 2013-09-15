// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Categories;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LeavinsSoftware.Collection.Program
{
    public partial class AddSubCategoryPage : Page
    {
        private AddSubCategoryViewModel model;

        public AddSubCategoryPage(ItemCategoryType categoryType)
        {
            InitializeComponent();
            model = new AddSubCategoryViewModel(new PageNavigationService(this), categoryType);
            DataContext = model;

            switch (model.MainCategory)
            {
                case ItemCategoryType.Product:
                    Title = InterfaceResources.AddSubCategory_Product;
                    break;
                case ItemCategoryType.ComicBook:
                    Title = InterfaceResources.AddSubCategory_ComicBook;
                    break;
                case ItemCategoryType.VideoGame:
                    Title = InterfaceResources.AddSubCategory_VideoGame;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Get unused categories
            IEnumerable<CategoryBase> categories = model.UnusedDefaultCategories;

            bool atLeastOneCategory = false;

            foreach (CategoryBase categoryBase in categories)
            {
                atLeastOneCategory = true;

                if (categoryBase.IsComposite)
                {
                    Expander categoryExpander = CreateCategoryGroup(categoryBase);
                    defaultCategoriesPanel.Children.Add(categoryExpander);
                }
                else
                {
                    Label categoryLabel = CreateCategoryLabel(categoryBase.ToItemCategory());
                    defaultCategoriesPanel.Children.Add(categoryLabel);
                }
            }

            switch (model.MainCategory)
            {
                case ItemCategoryType.Product:
                    addSubCategoryButton.Content = InterfaceResources.AddSubCategory_Product;
                    break;
                case ItemCategoryType.ComicBook:
                    addSubCategoryButton.Content = InterfaceResources.AddSubCategory_ComicBook;
                    break;
                case ItemCategoryType.VideoGame:
                    addSubCategoryButton.Content = InterfaceResources.AddSubCategory_VideoGame;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!atLeastOneCategory)
            {
                defaultCategoriesPanel.Visibility = Visibility.Collapsed;
            }
        }

        private Expander CreateCategoryGroup(CategoryBase categoryBase)
        {
            StackPanel categoriesWithHeaderPanel = new StackPanel();

            foreach (DefaultCategory innerCategory in categoryBase.Categories)
            {
                Label categoryLabel = CreateCategoryLabel(innerCategory.ToItemCategory());
                categoriesWithHeaderPanel.Children.Add(categoryLabel);
            }

            Expander categoryExpander = new Expander();
            categoryExpander.Header = categoryBase.Name;
            categoryExpander.Content = new GroupBox() { Content = categoriesWithHeaderPanel };
            return categoryExpander;
        }

        private Label CreateCategoryLabel(ItemCategory category)
        {
            Hyperlink categoryLink = new Hyperlink(new Run(category.Name));
            categoryLink.Command = model.CreateDefaultCategory;
            categoryLink.CommandParameter = category;
            Label categoryLabel = new Label();
            categoryLabel.Content = categoryLink;
            categoryLabel.Style = (Style)Application.Current.FindResource("categoryLabelStyle");
            return categoryLabel;
        }
    }
}
