// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.ViewModels;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for DeleteItemPage.xaml
    /// </summary>
    public partial class DeleteItemPage : Page
    {
        public class DeleteItemPageConfig
        {
            /// <summary>
            /// Number of pages to go back after successful deletion.
            /// </summary>
            /// <remarks>
            /// Default - 2
            /// </remarks>
            public int PagesToGoBack { get; set; }
        }

        private DeleteItemPage(DeleteItemPageConfig config = null)
        {
            InitializeComponent();
            Configuration = config;
        }
        
        public static DeleteItemPage Page<T>(T item, DeleteItemPageConfig config = null) where T : Item
        {
            var page = new DeleteItemPage(config);
            page.DataContext = DeleteItemViewModel.New(item, new PageNavigationService(page));
            return page;
        }
        
        public static DeleteItemPage PageForItemId<TItem>(long itemId, DeleteItemPageConfig config = null)
            where TItem : Item
        {
            var persistence = Persistence.GetInstance<IPersistence<TItem>>();
            
            return Page(persistence.Retrieve(itemId), config);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BasicMessenger.Default.Register(MessageIds.App_ItemDeleted, OnItemDeleted);
        }
        
        void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            BasicMessenger.Default.Unregister(MessageIds.App_ItemDeleted);
        }
        
        void OnItemDeleted()
        {
            var configInstance = Configuration ?? DefaultConfig;
            
            for(int x = 0; x < configInstance.PagesToGoBack; ++x)
            {
               NavigationService.GoBack();
            }
        }
        
        private static DeleteItemPageConfig DefaultConfig
        {
            get
            {
                return new DeleteItemPageConfig()
                {
                    PagesToGoBack = 2
                };
            }
        }
        
        private readonly DeleteItemPageConfig Configuration;
    }
}