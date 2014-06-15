// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using System;
using System.Collections;
using System.Linq;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="ItemGamePage"/>.
    /// </summary>
    public sealed class ItemGameViewModel : ViewModelBase
    {
        /// <summary>
        /// New Game constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="defaultCategory"></param>
        public ItemGameViewModel(IAppNavigationService nav, ItemCategory defaultCategory)
        {
            Item = new VideoGame()
            {
                System = defaultCategory
            };

            Setup(nav);
        }

        /// <summary>
        /// 'No selected system' constructor.
        /// </summary>
        /// <param name="nav"></param>
        public ItemGameViewModel(IAppNavigationService nav)
        {
            Item = new VideoGame();

            Setup(nav);
        }

        /// <summary>
        /// Existing Game constructor
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="gameId"></param>
        public ItemGameViewModel(IAppNavigationService nav, long gameId)
        {
            Item = Persistence.GetInstance<IVideoGamePersistence>().Retrieve(gameId);

            Setup(nav);
        }

        public IAppNavigationService Nav { get; private set; }

        public VideoGame Item { get; private set; }

        public IEnumerable Systems { get; private set; }

        public CustomCommand AddItem { get; private set; }
        
        public CustomCommand PromptDeleteItem { get; private set; }

        /// <summary>
        /// Method that is invoked to determine if page contents are valid.
        /// </summary>
        public Func<bool> PageValidator { get; set; }

        private void Setup(IAppNavigationService nav)
        {
            PageValidator = () => true;

            Nav = nav;

            AddItem = new CustomCommand(
                // Execute
                (x) =>
                {
                    Item.PropertyChanged -= Item_PropertyChanged;

                    if (Item.Id == 0)
                    {
                        Persistence.GetInstance<IVideoGamePersistence>().Create(Item);
                    }
                    else
                    {
                        Persistence.GetInstance<IVideoGamePersistence>().Update(Item);
                    }

                    Nav.GoBack();
                },

                // CanExecute
                (x) => Item.IsValid() && PageValidator.Invoke()
            );

            PromptDeleteItem = new CustomCommand(
                (x) => Nav.Navigate(() => DeleteItemPage.Page(Item)));
            
            Item.PropertyChanged += Item_PropertyChanged;

            Systems = Persistence.GetInstance<ICategoryPersistence>()
                .RetrieveAll(ItemCategoryType.VideoGame)
                .Select((p) => new { Name = p.Name, Value = p });
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddItem.TriggerCanExecuteChanged();
        }
    }
}
