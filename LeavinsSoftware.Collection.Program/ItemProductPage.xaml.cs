// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for ItemProductPage.xaml
    /// </summary>
    public partial class ItemProductPage : Page
    {
        private ItemProductViewModel model;

        public ItemProductPage()
        {
            InitializeComponent();
        }

        public static ItemProductPage NewProductPage()
        {
            var page = new ItemProductPage();

            page.Setup(new ItemProductViewModel(new PageNavigationService(page)));

            return page;
        }

        public static ItemProductPage NewProductPage(long categoryId)
        {
            ItemCategory category = Persistence.CategoryPersistence.Retrieve(categoryId);
            var page = new ItemProductPage();

            page.Setup(new ItemProductViewModel(new PageNavigationService(page),
                category));

            return page;
        }

        public static ItemProductPage ExistingProductPage(long productId)
        {
            var page = new ItemProductPage();

            page.Setup(new ItemProductViewModel(new PageNavigationService(page),
                productId));

            return page;
        }

        private void Setup(ItemProductViewModel context)
        {
            model = context;
            DataContext = model;

            model.PageValidator = () =>
                {
                    return !Validation.GetHasError(quantityBox);
                };

            if (model.Item.HasId)
            {
                Title = InterfaceResources.PageTitles_ItemExistingProduct;
            }
            else
            {
                Title = InterfaceResources.PageTitles_ItemNewProduct;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CommandManager.RequerySuggested += CommandManager_RequerySuggested;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CommandManager.RequerySuggested -= CommandManager_RequerySuggested;
        }

        void CommandManager_RequerySuggested(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.AddItem.TriggerCanExecuteChanged();
            }
        }
    }
}
