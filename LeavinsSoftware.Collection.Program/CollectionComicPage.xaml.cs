// Copyright (c) 2013 Dustin Leavins
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
    /// <summary>
    /// Interaction logic for CollectionComicPage.xaml
    /// </summary>
    [CollectionPage(ItemCategoryType.ComicBook)]
    public partial class CollectionComicPage : Page
    {
        public CollectionComicPage() : this(null)
        {
        }

        public CollectionComicPage(ItemCategory category)
        {
            InitializeComponent();
            var model = new CollectionComicViewModel(new PageNavigationService(this), category);
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
                    InterfaceResources.Common_ComicBooks,
                    model.SubCategory.Name);
            }
        }
    }
}
