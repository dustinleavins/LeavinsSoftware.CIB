// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class ItemComicViewModel : ViewModelBase
    {
        /// <summary>
        /// New Comic constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="defaultCategory"></param>
        public ItemComicViewModel(IAppNavigationService nav, ItemCategory defaultCategory)
        {
            Item = new ComicBook()
            {
                Publisher = defaultCategory
            };

            Setup(nav);
        }

        /// <summary>
        /// 'No selected publisher' constructor.
        /// </summary>
        /// <param name="nav"></param>
        public ItemComicViewModel(IAppNavigationService nav)
        {
            Item = new ComicBook();

            Setup(nav);
        }

        /// <summary>
        /// Existing Comic constructor
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="comicId"></param>
        public ItemComicViewModel(IAppNavigationService nav, long comicId)
        {
            Item = Persistence.ComicPersistence.Retrieve(comicId);

            Setup(nav);
        }

        public IAppNavigationService Nav { get; private set; }

        public ComicBook Item { get; private set; }

        public CustomCommand AddItem { get; private set; }

        public IEnumerable Publishers { get; private set; }

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
                    Item.Issues.CollectionChanged -= Issues_CollectionChanged;

                    if (Item.Id == 0)
                    {
                        Persistence.ComicPersistence.Create(Item);
                    }
                    else
                    {
                        Persistence.ComicPersistence.Update(Item);
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
            Item.Issues.CollectionChanged += Issues_CollectionChanged;

            Publishers = Persistence.CategoryPersistence
                .RetrieveAll(ItemCategoryType.ComicBook)
                .Select((p) => new { Name = p.Name, Value = p });
        }

        void Issues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AddItem.TriggerCanExecuteChanged();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddItem.TriggerCanExecuteChanged();
        }
    }
}
