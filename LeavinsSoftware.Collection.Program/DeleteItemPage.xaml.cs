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
        
        public static DeleteItemPage Page(ComicBookSeries book)
        {
            var page = new DeleteItemPage();
            page.DataContext = DeleteItemViewModel.New(book, new PageNavigationService(page));
            return page;
        }
        
        public static DeleteItemPage Page(VideoGame game)
        {
            var page = new DeleteItemPage();
            page.DataContext = DeleteItemViewModel.New(game, new PageNavigationService(page));
            return page;
        }
        
        public static DeleteItemPage Page(Product product)
        {
            var page = new DeleteItemPage();
            page.DataContext = DeleteItemViewModel.New(product, new PageNavigationService(page));
            return page;
        }
    }
}