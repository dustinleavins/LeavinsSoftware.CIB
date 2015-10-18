// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.Controls.ViewModels
{
    public sealed class SidebarViewModel : ViewModelBase
    {
        private List<MainCategoryItem> mainCategories;
        private MainCategoryItem selectedCategory;
        private ObservableCollection<ISidebarItem> sidebarItems;
        private ISidebarItem selectedSidebarItem;

        private SidebarViewModel(IAppNavigationService nav)
        {
            Nav = nav;
            sidebarItems = new ObservableCollection<ISidebarItem>();

            mainCategories = new List<MainCategoryItem>();

            foreach (ItemCategoryType categoryType in Enum.GetValues(typeof(ItemCategoryType)))
            {
                mainCategories.Add(new MainCategoryItem(categoryType, ItemResources.Get(categoryType).Name));
            }

            selectedCategory = mainCategories.FirstOrDefault();

            AddSubCategoryCommand = new CustomCommand(
                (_) =>
                {
                    if (selectedCategory == null)
                    {
                        return;
                    }

                    Nav.Navigate(() => new AddSubCategoryPage(selectedCategory.Type));
                });

            this.PropertyChanged += SidebarViewModel_PropertyChanged;
        }

        public IAppNavigationService Nav
        {
            get;
            private set;
        }

        public IEnumerable<MainCategoryItem> MainCategories
        {
            get
            {
                return mainCategories;
            }
        }

        public MainCategoryItem SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                if (selectedCategory != value)
                {
                    selectedCategory = value;
                    OnPropertyChanged("SelectedCategory");
                }
            }
        }

        public ISidebarItem SelectedSidebarItem
        {
            get
            {
                return selectedSidebarItem;
            }
            set
            {
                if (selectedSidebarItem != value)
                {
                    selectedSidebarItem = value;
                    OnPropertyChanged("SelectedSubCategory");

                    if (value != null)
                    {
                        Nav.Navigate(value.CreatePage);
                    }
                }
            }
        }

        public ObservableCollection<ISidebarItem> SidebarItems
        {
            get
            {
                return sidebarItems;
            }
        }

        public ICommand AddSubCategoryCommand
        {
            get;
            private set;
        }

        public async static Task<SidebarViewModel> Create(IAppNavigationService nav)
        {
            var vm = new SidebarViewModel(nav);
            await vm.UpdateSubCategories();
            return vm;
        }

        public async Task UpdateSubCategories()
        {
            sidebarItems.Clear();
            if (selectedCategory == null)
            {
                return;
            }

            var items = await Task.Run(
                () =>
                {
                    var categoryItems = new List<ISidebarItem>();
                    categoryItems.Add(new AllSubCategoriesItem(selectedCategory.Type));

                    foreach (var category in Persistence.GetInstance<ICategoryPersistence>().RetrieveAll(selectedCategory.Type))
                    {
                        categoryItems.Add(new SubCategoryItem(category, Nav));
                    }

                    return categoryItems;
                });

            foreach (var item in items)
            {
                sidebarItems.Add(item);
            }
        }

        private async void SidebarViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bool updateSubCategories = string.Equals("SelectedCategory", e.PropertyName, StringComparison.Ordinal);

            if (updateSubCategories)
            {
                await UpdateSubCategories();
            }
        }

        public sealed class MainCategoryItem
        {
            public MainCategoryItem(ItemCategoryType type, string displayName)
            {
                Type = type;
                DisplayName = displayName;
            }

            public ItemCategoryType Type
            {
                get;
                private set;
            }

            public string DisplayName
            {
                get;
                private set;
            }
        }

        public sealed class ListTypeItem
        {
            public ListTypeItem(ItemListType listType, string displayName)
            {
                ListType = listType;
                DisplayName = displayName;
            }

            public ItemListType ListType
            {
                get;
                private set;
            }

            public string DisplayName
            {
                get;
                private set;
            }
        }

        public interface ISidebarItem
        {
            string Name { get; }
            Page CreatePage();
        }

        public sealed class AllSubCategoriesItem : ISidebarItem
        {
            public AllSubCategoriesItem(ItemCategoryType categoryType)
            {
                CategoryType = categoryType;
            }

            public ItemCategoryType CategoryType
            {
                get;
                private set;
            }

            public string Name
            {
                get
                {
                    return InterfaceResources.Common_ViewAll;
                }
            }

            public Page CreatePage()
            {
                return CollectionPage.PageFor(CategoryType);
            }
        }

        public sealed class SubCategoryItem : ISidebarItem
        {
            public SubCategoryItem(ItemCategory category, IAppNavigationService nav)
            {
                Category = category;
                Nav = nav;
            }

            public IAppNavigationService Nav
            {
                get;
                private set;
            }

            public string Name
            {
                get
                {
                    return Category.Name;
                }
            }

            public ItemCategory Category
            {
                get;
                private set;
            }

            public Page CreatePage()
            {
                return CollectionPage.PageFor(Category);
            }
        }
    }
}
