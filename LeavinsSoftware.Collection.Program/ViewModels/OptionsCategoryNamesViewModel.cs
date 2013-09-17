// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="OptionsCategoryNamesPage"/>.
    /// </summary>
    public sealed class OptionsCategoryNamesViewModel : ViewModelBase
    {
        public OptionsCategoryNamesViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            SaveNames = new CustomCommand(
                (x) =>
                {
                    List<ItemCategory> updatedCategories = new List<ItemCategory>();

                    foreach (ItemCategory category in Categories)
                    {
                        string originalName = originalCategoryNames[category.Id];

                        if (!string.Equals(category.Name, originalName, StringComparison.Ordinal))
                        {
                            updatedCategories.Add(category);
                        }
                    }

                    foreach (ItemCategory updatedCategory in updatedCategories)
                    {
                        Persistence.CategoryPersistence.Update(updatedCategory);
                    }

                    Nav.GoBack();
                },

                (x) =>
                {
                    return !(Categories.Any(c => !c.IsValid()));
                });

            Categories = Persistence.CategoryPersistence.RetrieveAll();

            originalCategoryNames = Categories.ToDictionary(
                (cat) => cat.Id,
                (cat) => cat.Name);
        }

        public CustomCommand SaveNames { get; private set; }

        public IAppNavigationService Nav { get; private set; }

        public IEnumerable<ItemCategory> Categories { get; private set; }

        public void OnLoaded()
        {
            foreach (ItemCategory category in Categories)
            {
                category.PropertyChanged += CategoryChanged;
            }
        }

        public void OnUnloaded()
        {
            foreach (ItemCategory category in Categories)
            {
                category.PropertyChanged -= CategoryChanged;
            }
        }

        void CategoryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveNames.TriggerCanExecuteChanged();
        }

        private IDictionary<long, string> originalCategoryNames;
    }
}
