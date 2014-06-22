// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LeavinsSoftware.Collection.Program.Resources;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for Category pages.
    /// </summary>
    public sealed class CategoryViewModel
    {
        public static CategoryViewModel Create<TItem>(IAppNavigationService nav) where TItem : Item
        {
            return new CategoryViewModel(nav, Item.CategoryType<TItem>());
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

            GoToAll = new CustomCommand(
                (x) => Nav.Navigate(() => CollectionPage.PageFor(primaryCategory)));

            AddCategory = new CustomCommand(
                (unused) => Nav.Navigate(() => new AddSubCategoryPage(primaryCategory)));
        }

        public IAppNavigationService Nav { get; private set; }

        public ICommand GoTo { get; private set; }

        public ICommand GoToAll { get; private set; }

        public ICommand AddCategory { get; private set; }

        public ObservableCollection<ItemCategory> Categories { get; private set; }

        public String AddCategoryText
        {
            get
            {
                return ItemResources.Get(primaryCategory).AddSubCategory;
            }
        }

        public void OnLoaded()
        {
            Categories.Clear();

            ICollection<ItemCategory> categoriesToAdd = Persistence
                .GetInstance<ICategoryPersistence>()
                .RetrieveAll(primaryCategory);

            foreach (ItemCategory category in categoriesToAdd)
            {
                Categories.Add(category);
            }
        }

        private ItemCategoryType primaryCategory;
    }
}
