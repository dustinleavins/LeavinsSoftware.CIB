// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="CollectionGamePage"/>.
    /// </summary>
    public sealed class CollectionGameViewModel : CollectionViewModelBase<VideoGame>
    {
        public CollectionGameViewModel(IAppNavigationService nav, ItemCategory subCategory) :
            base(nav, subCategory)
        {
            AddItem = new CustomCommand((x) =>
            {
                if (SubCategory == null)
                {
                    Nav.Navigate(() => ItemGamePage.NewGamePage());
                }
                else
                {
                    Nav.Navigate(() => ItemGamePage.NewGamePage(SubCategory.Id));
                }
            });

            EditSelectedItem = new CustomCommand(
                // Execute
                (x) => Nav.Navigate(() => ItemGamePage.ExistingGamePage(SelectedItem.Id)),

                // CanExecute
                (x) => SelectedItem != null
            );
            
            DeleteSelectedItem = new CustomCommand(
                (x) =>
                {
                    var config = new DeleteItemPage.DeleteItemPageConfig()
                    {
                        PagesToGoBack = 1
                    };
                    
                    Nav.Navigate(() => DeleteItemPage.PageForItemId<VideoGame>(SelectedItem.Id, config));
                });
        }

        public override ICommand AddItem { get; protected set; }

        public override CustomCommand EditSelectedItem { get; protected set; }
        
        public override CustomCommand DeleteSelectedItem { get; protected set; }
    }
}
