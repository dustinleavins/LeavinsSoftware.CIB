// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for Category pages.
    /// </summary>
    public sealed class CategoryViewModel
    {
        public static CategoryViewModel Product(IAppNavigationService nav)
        {
            return new CategoryViewModel(nav, ItemCategoryType.Product);
        }

        public static CategoryViewModel ComicBook(IAppNavigationService nav)
        {
            return new CategoryViewModel(nav, ItemCategoryType.ComicBook);
        }

        public static CategoryViewModel VideoGame(IAppNavigationService nav)
        {
            return new CategoryViewModel(nav, ItemCategoryType.VideoGame);
        }

        private CategoryViewModel(IAppNavigationService nav, ItemCategoryType category)
        {
            primaryCategory = category;
            Nav = nav;

            Categories = new ObservableCollection<ItemCategory>();

            GoTo = new CustomCommand((paramObject) =>
                {
                    ItemCategory paramAsCategory = paramObject as ItemCategory;
                    Nav.Navigate(() => CollectionPage.PageFor(paramAsCategory));
                });

            GoToAll = new CustomCommand((x) =>
            {
                Nav.Navigate(() => CollectionPage.PageFor(primaryCategory));
            });

            AddCategory = new CustomCommand((unused) =>
            {
                Nav.Navigate(() => new AddSubCategoryPage(primaryCategory));
            });
        }

        public IAppNavigationService Nav { get; private set; }

        public ICommand GoTo { get; private set; }

        public ICommand GoToAll { get; private set; }

        public ICommand AddCategory { get; private set; }

        public ObservableCollection<ItemCategory> Categories { get; private set; }

        public void OnLoaded()
        {
            Categories.Clear();

            ICollection<ItemCategory> categoriesToAdd = Persistence
                .CategoryPersistence
                .RetrieveAll(primaryCategory);

            foreach (ItemCategory category in categoriesToAdd)
            {
                Categories.Add(category);
            }
        }

        private ItemCategoryType primaryCategory;
    }
}
