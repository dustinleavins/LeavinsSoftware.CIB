﻿// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
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
            ItemCategory category = Persistence.GetInstance<ICategoryPersistence>().Retrieve(categoryId);
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
                !Validation.GetHasError(quantityBox);

            if (model.Item.IsNew)
            {
                Title = InterfaceResources.PageTitles_ItemNewProduct;
                deleteBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                Title = InterfaceResources.PageTitles_ItemExistingProduct;
            }
        }

        void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (model.Item.IsNew)
            {
                nameTextBox.Focus();
            }
            
            BasicMessenger.Default.Register(MessageIds.App_Finish, OnFinish);
        }
        
        void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BasicMessenger.Default.Unregister(MessageIds.App_Finish);
        }
        
        private void OnFinish()
        {
            if (model.AddItem.CanExecute(null))
            {
                model.AddItem.Execute(null);
            }
        }
    }
}
