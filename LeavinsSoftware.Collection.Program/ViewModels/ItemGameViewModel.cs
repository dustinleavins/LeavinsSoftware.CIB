// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections;
using System.Linq;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
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
        /// <param name="GameId"></param>
        public ItemGameViewModel(IAppNavigationService nav, long GameId)
        {
            Item = Persistence.GamePersistence.Retrieve(GameId);

            Setup(nav);
        }

        public IAppNavigationService Nav { get; private set; }

        public VideoGame Item { get; private set; }

        public IEnumerable Systems { get; private set; }

        public CustomCommand AddItem { get; private set; }

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
                        Persistence.GamePersistence.Create(Item);
                    }
                    else
                    {
                        Persistence.GamePersistence.Update(Item);
                    }

                    Nav.GoBack();
                },

                // CanExecute
                (x) =>
                {
                    return Item.IsValid() && PageValidator.Invoke();
                }
            );

            Item.PropertyChanged += Item_PropertyChanged;

            Systems = Persistence.CategoryPersistence
                .RetrieveAll(ItemCategoryType.VideoGame)
                .Select((p) => new { Name = p.Name, Value = p });
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddItem.TriggerCanExecuteChanged();
        }
    }
}
