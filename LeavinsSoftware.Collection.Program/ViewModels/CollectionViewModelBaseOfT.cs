// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// Base class for collection pages
    /// </summary>
    /// <typeparam name="TSearchable"></typeparam>
    public abstract class CollectionViewModelBase<TSearchable> : ViewModelBase where TSearchable : Item
    {
        public CollectionViewModelBase(IAppNavigationService nav, ItemCategory subCategory)
        {
            Nav = nav;
            SubCategory = subCategory;

            CurrentPage = new ObservableCollection<TSearchable>();

            PreviousPage = new CustomCommand();
            NextPage = new CustomCommand();

            PreviousPage.ExecuteHandler = (x) =>
            {
                currentSearch.PreviousPage();
                RefreshSearch();
            };

            PreviousPage.CanExecuteHandler = (x) =>
            {
                return currentSearch != null &&
                    currentSearch.HasPreviousPage;
            };

            NextPage.ExecuteHandler = (x) =>
            {
                currentSearch.NextPage();
                RefreshSearch();
            };

            NextPage.CanExecuteHandler = (x) =>
            {
                return currentSearch != null &&
                    currentSearch.HasNextPage;
            };

            Search = new CustomCommand(
                (x) =>
                {
                    NewSearch();
                });
        }

        public IAppNavigationService Nav { get; private set; }

        public ItemCategory SubCategory { get; private set; }

        public abstract ICommand AddItem { get; protected set; }

        public ICommand Search { get; private set; }

        public abstract CustomCommand EditSelectedItem { get; protected set; }

        public CustomCommand PreviousPage { get; private set; }

        public CustomCommand NextPage { get; private set; }

        public ObservableCollection<TSearchable> CurrentPage
        {
            get;
            private set;
        }

        public TSearchable SelectedItem
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

                    NewSearch();
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

        private void NewSearch()
        {
            currentSearch = new Search<TSearchable>(
                Persistence.GetInstance<ISearchablePersistence<TSearchable>>(),
                new ModelSearchOptionsBuilder()
                {
                    ItemCategory = SubCategory,
                    ItemsPerPage = 20,
                    ListType = ListType,
                    SearchText = SearchText
                }.Build());

            RefreshSearch();
        }

        public void OnLoaded()
        {
            NewSearch();
        }

        private void RefreshSearch()
        {
            CurrentPage.Clear();

            foreach (TSearchable result in currentSearch.CurrentPage)
            {
                CurrentPage.Add(result);
            }

            NextPage.TriggerCanExecuteChanged();
            PreviousPage.TriggerCanExecuteChanged();
        }

        private Search<TSearchable> currentSearch;
        private TSearchable selectedItem;
        private ItemListType listType;
        private string searchText;
    }
}
