﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Globalization;
using System.Windows.Controls;
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Attributes;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;

namespace LeavinsSoftware.Collection.Program
{
    [CollectionPage(ItemCategoryType.Product)]
    public partial class CollectionProductPage : Page
    {
        public CollectionProductPage()
            : this(null)
        {
        }

        public CollectionProductPage(ItemCategory category)
        {
            InitializeComponent();
            var model = new CollectionProductViewModel(new PageNavigationService(this), category);
            DataContext = model;

            Loaded += (x, y) =>
                {
                    model.OnLoaded();
                };

            if (model.SubCategory != null)
            {
                Title = string.Format(
                    CultureInfo.InvariantCulture,
                    InterfaceResources.PageTitles_CollectionFormat,
                    InterfaceResources.Common_Products,
                    model.SubCategory.Name);
            }
        }
    }
}
