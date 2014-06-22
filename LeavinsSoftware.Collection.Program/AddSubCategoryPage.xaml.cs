// Copyright (c) 2013, 2014 Dustin Leavins
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
using System.Windows.Media;

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

            Title = ItemResources
                .Get(model.MainCategory)
                .AddSubCategory;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            model.OnLoaded();
            // Get unused categories
            IEnumerable<IDefaultCategory> categories = model.UnusedDefaultCategories;

            bool atLeastOneCategory = false;

            foreach (IDefaultCategory categoryBase in categories)
            {
                atLeastOneCategory = true;

                if (categoryBase.IsComposite)
                {
                    Expander categoryExpander = CreateCategoryGroup(categoryBase);
                    defaultCategoriesPanel.Children.Add(categoryExpander);
                }
                else
                {
                    Label categoryLabel = CreateCategoryLabel(categoryBase);
                    defaultCategoriesPanel.Children.Add(categoryLabel);
                }
            }

            addSubCategoryButton.Content = ItemResources
                .Get(model.MainCategory)
                .AddSubCategory;

            if (!atLeastOneCategory)
            {
                defaultCategoriesPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            model.OnUnloaded();
        }

        private Expander CreateCategoryGroup(IDefaultCategory categoryBase)
        {
            StackPanel categoriesWithHeaderPanel = new StackPanel();

            foreach (DefaultCategory innerCategory in categoryBase.Categories)
            {
                Label categoryLabel = CreateCategoryLabel(innerCategory);
                categoriesWithHeaderPanel.Children.Add(categoryLabel);
            }

            Expander categoryExpander = new Expander();
            categoryExpander.Header = categoryBase.Name;
            Border categoryBorder = new Border()
            {
                Child = categoriesWithHeaderPanel,
                Style = (Style)Application.Current.FindResource("categoryBorderStyle")
            };

            categoryExpander.Content = categoryBorder;

            categoryExpander.Style = (Style)Application.Current.FindResource("categoryExpanderStyle");

            return categoryExpander;
        }
        
        private Label CreateCategoryLabel(IDefaultCategory category)
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
