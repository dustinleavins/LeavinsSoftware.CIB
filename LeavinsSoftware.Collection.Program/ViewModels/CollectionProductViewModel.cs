﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF.ViewModel;
using KSMVVM.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeavinsSoftware.Collection.Models;
using System.Windows.Input;
using LeavinsSoftware.Collection.Persistence;
using System.Collections.ObjectModel;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class CollectionProductViewModel : ViewModelBase
    {
        public CollectionProductViewModel(IAppNavigationService nav, ItemCategory subCategory)
        {
            Nav = nav;
            SubCategory = subCategory;

            AddItem = new CustomCommand((x) =>
                {
                    if (SubCategory == null)
                    {
                        Nav.Navigate(() => ItemProductPage.NewProductPage());
                    }
                    else
                    {
                        Nav.Navigate(() => ItemProductPage.NewProductPage(SubCategory.Id));
                    }
                });

            EditSelectedItem = new CustomCommand(
                // Execute
                (x) =>
                {
                    Nav.Navigate(() => ItemProductPage.ExistingProductPage(SelectedItem.Id));
                },

                // CanExecute
                (x) =>
                {
                    return SelectedItem != null;
                }
            );

            CurrentPage = new ObservableCollection<Product>();

            PreviousPage = new CustomCommand();
            NextPage = new CustomCommand();

            PreviousPage.ExecuteHandler = (x) =>
                {
                    currentSearch.PreviousPage();
                    RefreshSearch();
                };

            PreviousPage.CanExecuteHandler = (x) =>
                {
                    return currentSearch.HasPreviousPage;
                };

            NextPage.ExecuteHandler = (x) =>
                {
                    currentSearch.NextPage();
                    RefreshSearch();
                };

            NextPage.CanExecuteHandler = (x) =>
            {
                return currentSearch.HasNextPage;
            };

            Search = new CustomCommand(
                (x) =>
                {
                    // TODO: Refactor
                    currentSearch = new Search<Product>(Persistence.ProductPersistence, new ModelSearchOptionsBuilder()
                    {
                        ItemCategory = SubCategory,
                        ItemsPerPage = 20,
                        ListType = ListType,
                        SearchText = SearchText
                    }.Build());

                    RefreshSearch();
                });
        }

        public IAppNavigationService Nav { get; private set; }

        public ItemCategory SubCategory { get; private set; }

        public ICommand AddItem { get; private set; }

        public ICommand Search { get; private set; }

        public CustomCommand EditSelectedItem { get; private set; }

        public CustomCommand PreviousPage { get; private set; }

        public CustomCommand NextPage { get; private set; }

        public ObservableCollection<Product> CurrentPage
        {
            get;
            private set;
        }

        public Product SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged("SelectedItem");
                    EditSelectedItem.TriggerCanExecuteChanged();
                }
            }
        }

        public ItemListType ListType
        {
            get
            {
                return listType;
            }
            set
            {
                if (listType != value)
                {
                    listType = value;
                    OnPropertyChanged("ListType");

                    // TODO: Refactor
                    currentSearch = new Search<Product>(Persistence.ProductPersistence, new ModelSearchOptionsBuilder()
                    {
                        ItemCategory = SubCategory,
                        ItemsPerPage = 20,
                        ListType = ListType,
                        SearchText = SearchText
                    }.Build());

                    RefreshSearch();
                }
            }
        }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged("SearchText");
                }
            }
        }

        public void OnLoaded()
        {
            currentSearch = new Search<Product>(Persistence.ProductPersistence, new ModelSearchOptionsBuilder()
            {
                ItemCategory = SubCategory,
                ItemsPerPage = 20,
                ListType = ListType,
                SearchText = SearchText
            }.Build());

            RefreshSearch();
        }

        private void RefreshSearch()
        {
            CurrentPage.Clear();

            foreach (Product result in currentSearch.CurrentPage)
            {
                CurrentPage.Add(result);
            }

            NextPage.TriggerCanExecuteChanged();
            PreviousPage.TriggerCanExecuteChanged();
        }

        private Search<Product> currentSearch;
        private Product selectedItem;
        private ItemListType listType;
        private string searchText;
    }
}
