// Copyright (c) 2013, 2014 Dustin Leavins
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
    /// ViewModel for <see cref="ItemProductPage"/>.
    /// </summary>
    public sealed class ItemProductViewModel : ViewModelBase
    {
        /// <summary>
        /// New Product constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="defaultCategory"></param>
        public ItemProductViewModel(IAppNavigationService nav, ItemCategory defaultCategory)
        {
            Item = new Product()
            {
                Category = defaultCategory
            };

            Setup(nav);
        }

        /// <summary>
        /// 'No selected category' constructor.
        /// </summary>
        /// <param name="nav"></param>
        public ItemProductViewModel(IAppNavigationService nav)
        {
            Item = new Product();

            Setup(nav);
        }

        /// <summary>
        /// Existing product constructor
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="productId"></param>
        public ItemProductViewModel(IAppNavigationService nav, long productId)
        {
            Item = Persistence.GetInstance<IProductPersistence>().Retrieve(productId);

            Setup(nav);
        }

        public IAppNavigationService Nav { get; private set; }

        public Product Item { get; private set; }

        /// <summary>
        /// Method that is invoked to determine if page contents are valid.
        /// </summary>
        public Func<bool> PageValidator { get; set; }

        public CustomCommand AddItem { get; private set; }

        public IEnumerable Categories { get; private set; }

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
                        Persistence.GetInstance<IProductPersistence>().Create(Item);
                    }
                    else
                    {
                        Persistence.GetInstance<IProductPersistence>().Update(Item);
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

            Categories = Persistence.GetInstance<ICategoryPersistence>()
                .RetrieveAll(ItemCategoryType.Product)
                .Select((p) => new { Name = p.Name, Value = p });
        }

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddItem.TriggerCanExecuteChanged();
        }
    }
}
