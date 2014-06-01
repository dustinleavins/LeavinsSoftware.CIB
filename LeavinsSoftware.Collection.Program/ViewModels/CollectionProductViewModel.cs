// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="CollectionProductPage"/>.
    /// </summary>
    public sealed class CollectionProductViewModel : CollectionViewModelBase<Product>
    {
        public CollectionProductViewModel(IAppNavigationService nav, ItemCategory subCategory) :
            base(nav, subCategory)
        {
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
        }

        public override ICommand AddItem { get; protected set; }

        public override CustomCommand EditSelectedItem { get; protected set; }
    }
}
