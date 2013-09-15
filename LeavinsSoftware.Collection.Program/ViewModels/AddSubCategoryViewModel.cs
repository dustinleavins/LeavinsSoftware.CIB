// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class AddSubCategoryViewModel : ViewModelBase
    {
        public AddSubCategoryViewModel(IAppNavigationService nav, ItemCategoryType mainCategory)
        {
            Nav = nav;

            MainCategory = mainCategory;

            IEnumerable<string> persistedCodes = Persistence
                .CategoryPersistence
                .RetrieveAll(mainCategory)
                .Select(c => c.Code);

            UnusedDefaultCategories = DefaultCategories.CategoriesFor(mainCategory)
                .Where(defaultCategory => !persistedCodes.Contains(defaultCategory.Code));

            AddSubCategory = new CustomCommand(
                // Execute
                (unused) =>
                {
                    PropertyChanged -= PropertyChangedHandler;

                    ItemCategory newCategory = new ItemCategory()
                    {
                        Name = SubCategoryName,
                        CategoryType = MainCategory
                    };

                    Persistence.CategoryPersistence.Create(newCategory);
                    Nav.Navigate(() => CollectionPage.PageFor(newCategory));
                },

                // CanExecute
                (unused) =>
                {
                    return !string.IsNullOrWhiteSpace(SubCategoryName);
                }
            );

            CreateDefaultCategory = new CustomCommand(
                (categoryObject) =>
                {
                    PropertyChanged -= PropertyChangedHandler;

                    ItemCategory newCategory = categoryObject as ItemCategory;

                    Persistence.CategoryPersistence.Create(newCategory);
                    Nav.Navigate(() => CollectionPage.PageFor(newCategory));
                });

            PropertyChanged += PropertyChangedHandler;
        }

        void PropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddSubCategory.TriggerCanExecuteChanged();
        }

        public IAppNavigationService Nav { get; private set; }

        public ItemCategoryType MainCategory { get; private set; }

        public CustomCommand AddSubCategory { get; private set; }

        public ICommand CreateDefaultCategory { get; private set; }

        public IEnumerable<ItemCategory> UnusedDefaultCategories { get; private set; }

        public string SubCategoryName
        {
            get
            {
                return subCategoryName;
            }
            set
            {
                if (subCategoryName != value)
                {
                    subCategoryName = value;
                    OnPropertyChanged("SubCategoryName");
                }
            }
        }

        private string subCategoryName;
    }
}
