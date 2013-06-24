﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class CategoryGameViewModel : ViewModelBase
    {
        public CategoryGameViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            Categories = new ObservableCollection<ItemCategory>();

            GoTo = new CustomCommand((paramObject) =>
            {
                ItemCategory paramAsCategory = paramObject as ItemCategory;
                Nav.Navigate(() => new CollectionGamePage(paramAsCategory));
            });

            GoToAll = new CustomCommand((x) =>
            {
                Nav.Navigate(() => new CollectionGamePage());
            });

            AddCategory = new CustomCommand((unused) =>
            {
                Nav.Navigate(() => new AddSubCategoryPage(ItemCategoryType.VideoGame));
            });
        }

        public IAppNavigationService Nav { get; private set; }

        public ICommand GoTo { get; private set; }

        public ICommand GoToAll { get; private set; }

        public ICommand AddCategory { get; private set; }

        public ObservableCollection<ItemCategory> Categories { get; private set; }

        public void LoadedHandler()
        {
            Categories.Clear();

            ICollection<ItemCategory> categoriesToAdd = Persistence
                .CategoryPersistence
                .RetrieveAll(ItemCategoryType.VideoGame);

            foreach (ItemCategory category in categoriesToAdd)
            {
                Categories.Add(category);
            }
        }
    }
}
