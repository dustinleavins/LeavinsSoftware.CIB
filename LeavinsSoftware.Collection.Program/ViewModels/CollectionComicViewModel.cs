﻿// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="CollectionComicPage"/>.
    /// </summary>
    public sealed class CollectionComicViewModel : CollectionViewModelBase<ComicBookSeries>
    {
        public CollectionComicViewModel(IAppNavigationService nav, ItemCategory subCategory) :
            base(nav, subCategory)
        {
            AddItem = new CustomCommand((x) =>
            {
                if (SubCategory == null)
                {
                    Nav.Navigate(() => ItemComicPage.NewComicPage());
                }
                else
                {
                    Nav.Navigate(() => ItemComicPage.NewComicPage(SubCategory.Id));
                }
            });

            EditSelectedItem = new CustomCommand(
                // Execute
                (x) => Nav.Navigate(() => ItemComicPage.ExistingComicPage(SelectedItem.Id)),

                // CanExecute
                (x) => SelectedItem != null
            );
        }

        public override ICommand AddItem { get; protected set; }

        public override CustomCommand EditSelectedItem { get; protected set; }
    }
}
