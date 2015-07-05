// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                        Persistence.GetInstance<ICategoryPersistence>().Update(updatedCategory);
                    }

                    Nav.GoBack();
                },

                (x) => Categories.All(c => c.IsValid()));

            Categories = new BindingList<ItemCategory>();
            foreach (var category in Persistence.GetInstance<ICategoryPersistence>().RetrieveAll())
            {
                Categories.Add(category);
            }

            Categories.ListChanged += Categories_ListChanged;

            originalCategoryNames = Categories.ToDictionary(
                (cat) => cat.Id,
                (cat) => cat.Name);
        }

        public CustomCommand SaveNames { get; private set; }

        public IAppNavigationService Nav { get; private set; }

        public BindingList<ItemCategory> Categories { get; private set; }

        private void Categories_ListChanged(object sender, ListChangedEventArgs e)
        {
            SaveNames.TriggerCanExecuteChanged();
        }

        private IDictionary<long, string> originalCategoryNames;
    }
}
