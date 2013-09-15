// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Categories;
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

            //UnusedDefaultCategories = DefaultCategories.CategoriesFor(mainCategory)
            //    .Where(defaultCategory => !persistedCodes.Contains(defaultCategory.Code));

            // Initialize UnusedDefaultCategories
            List<CategoryBase> unusedCategories = new List<CategoryBase>();
            foreach (CategoryBase category in DefaultCategories.CategoriesFor(mainCategory))
            {
                if (category.IsComposite)
                {
                    List<DefaultCategory> unusedInnerCategories = new List<DefaultCategory>();

                    foreach (DefaultCategory innerCategory in category.Categories)
                    {
                        if (!persistedCodes.Contains(innerCategory.ToItemCategory().Code))
                        {
                            unusedInnerCategories.Add(innerCategory);
                        }
                    }

                    if (unusedInnerCategories.Count > 0)
                    {
                        CompositeCategory copiedCategory = new CompositeCategory(unusedInnerCategories)
                        {
                            Name = category.Name,
                            CategoryType = category.CategoryType
                        };

                        unusedCategories.Add(copiedCategory);
                    }
                }
                else if (!persistedCodes.Contains(category.ToItemCategory().Code))
                {
                    unusedCategories.Add(category);
                }
            }

            UnusedDefaultCategories = unusedCategories;

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

        public IEnumerable<CategoryBase> UnusedDefaultCategories { get; private set; }

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
