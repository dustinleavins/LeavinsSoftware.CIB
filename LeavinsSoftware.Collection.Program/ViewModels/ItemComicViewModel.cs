// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="ItemComicPage"/>.
    /// </summary>
    public sealed class ItemComicViewModel : ViewModelBase
    {
        /// <summary>
        /// New Comic constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="defaultCategory"></param>
        public ItemComicViewModel(IAppNavigationService nav, ItemCategory defaultCategory)
        {
            Item = new ComicBookSeries()
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
            Item = new ComicBookSeries();

            Setup(nav);
        }

        /// <summary>
        /// Existing Comic constructor
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="comicId"></param>
        public ItemComicViewModel(IAppNavigationService nav, long comicId)
        {
            Item = Persistence.GetInstance<IComicBookPersistence>().Retrieve(comicId);

            Setup(nav);
        }

        public IAppNavigationService Nav { get; private set; }

        public ComicBookSeries Item { get; private set; }

        public CustomCommand AddItem { get; private set; }

        public IEnumerable Publishers { get; private set; }

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
                    Item.Entries.CollectionChanged -= Issues_CollectionChanged;

                    if (Item.Id == 0)
                    {
                        Persistence.GetInstance<IComicBookPersistence>().Create(Item);
                    }
                    else
                    {
                        Persistence.GetInstance<IComicBookPersistence>().Update(Item);
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
            Item.Entries.CollectionChanged += Issues_CollectionChanged;

            Publishers = Persistence.GetInstance<ICategoryPersistence>()
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
