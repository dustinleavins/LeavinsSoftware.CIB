// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for ItemGamePage.xaml
    /// </summary>
    public partial class ItemGamePage : Page
    {
        private ItemGameViewModel model;

        public ItemGamePage()
        {
            InitializeComponent();
        }

        public static ItemGamePage NewGamePage()
        {
            var page = new ItemGamePage();

            page.Setup(new ItemGameViewModel(new PageNavigationService(page)));

            return page;
        }

        public static ItemGamePage NewGamePage(long categoryId)
        {
            ItemCategory category = Persistence.GetInstance<ICategoryPersistence>().Retrieve(categoryId);
            var page = new ItemGamePage();

            page.Setup(new ItemGameViewModel(new PageNavigationService(page),
                category));

            return page;
        }

        public static ItemGamePage ExistingGamePage(long gameId)
        {
            var page = new ItemGamePage();

            page.Setup(new ItemGameViewModel(new PageNavigationService(page),
                gameId));

            return page;
        }

        private void Setup(ItemGameViewModel context)
        {
            model = context;
            DataContext = model;

            model.Item.PropertyChanged += Item_PropertyChanged;
            SetupFields();

            if (model.Item.IsNew)
            {
                Title = InterfaceResources.PageTitles_ItemNewVideoGame; 
                deleteBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                Title = InterfaceResources.PageTitles_ItemExistingVideoGame;
            }
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DistributionType")
            {
                return;
            }

            SetupFields();
        }

        private void SetupFields()
        {
            switch (model.Item.DistributionType)
            {
                case DistributionType.Digital:
                    conditionLabel.Visibility = Visibility.Collapsed;
                    conditionBox.Visibility = Visibility.Collapsed;
                    providerLabel.Visibility = Visibility.Visible;
                    ProviderBox.Visibility = Visibility.Visible;
                    break;
                case DistributionType.Physical:
                    conditionLabel.Visibility = Visibility.Visible;
                    conditionBox.Visibility = Visibility.Visible;
                    providerLabel.Visibility = Visibility.Collapsed;
                    ProviderBox.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        
        void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (model.Item.IsNew)
            {
                nameTextBox.Focus();
            }
            
            BasicMessenger.Default.Register(MessageIds.App_Finish, OnFinish);
        }
        
        void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BasicMessenger.Default.Unregister(MessageIds.App_Finish);
        }
        
        private void OnFinish()
        {
            if (model.AddItem.CanExecute(null))
            {
                model.AddItem.Execute(null);
            }
        }
    }
}
