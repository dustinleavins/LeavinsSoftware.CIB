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
using LeavinsSoftware.Collection.Program.ViewModels;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for DeleteItemPage.xaml
    /// </summary>
    public partial class DeleteItemPage : Page
    {
        private DeleteItemPage()
        {
            InitializeComponent();
        }
        
        public static DeleteItemPage Page<T>(T item) where T : Item
        {
            var page = new DeleteItemPage();
            page.DataContext = DeleteItemViewModel.New(item, new PageNavigationService(page));
            return page;
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
            NavigationService.GoBack();
            NavigationService.GoBack();
        }
    }
}