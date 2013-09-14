﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Attributes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CollectionPageAttribute collectAttribute = mainFrame.Content
                .GetType()
                .GetCustomAttributes(true)
                .SingleOrDefault(o => o.GetType() == typeof(CollectionPageAttribute))
                as CollectionPageAttribute;

            if (collectAttribute == null)
            {
                mainFrame.GoBack();
                e.Handled = true;
                return;
            }

            bool hasAddSubPage = false;
            bool hasAddMainPage = false;

            foreach (JournalEntry stackEntry in mainFrame.BackStack)
            {
                if (stackEntry.Name == "addSubCategoryPage")
                {
                    hasAddSubPage = true;
                }
                else if (stackEntry.Name == "addMainCategoryPage")
                {
                    hasAddMainPage = true;
                }
            }

            if (hasAddSubPage && hasAddMainPage)
            {
                mainFrame.RemoveBackEntry(); // Remove category
                mainFrame.RemoveBackEntry(); // Remove 'add sub'

                Page categoryPage = null;

                switch (collectAttribute.CategoryType)
                {
                    case ItemCategoryType.ComicBook:
                        categoryPage = new CategoryComicPage();
                        break;
                    case ItemCategoryType.Product:
                        categoryPage = new CategoryProductPage();
                        break;
                    case ItemCategoryType.VideoGame:
                        categoryPage = new CategoryGamePage();
                        break;
                    default:
                        throw new NotSupportedException();
                }

                mainFrame.Navigate(categoryPage);

                // Setup nav workaround
                // At some point between this BrowseBack handler and mainFrame.Navigated,
                // the current page (which is one of the collection pages) gets added
                // to mainFrame's back entries.
                // Setting doNavWorkaround informs the mainFrame.Navigated handler in
                // this file to remove this undesired entry.
                doNavWorkaround = true;
            }
            else if (hasAddSubPage)
            {
                mainFrame.RemoveBackEntry();
                mainFrame.GoBack();
            }
            else
            {
                mainFrame.GoBack();
            }

            e.Handled = true;
        }

        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mainFrame.CanGoBack;
        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (doNavWorkaround)
            {
                mainFrame.RemoveBackEntry(); // This should remove the 'Collection' page from history
                doNavWorkaround = false;
            }

            headerLabel.Content = (e.Content as Page).Title;

            if (mainFrame.CanGoBack)
            {
                backLinkRectangle.Fill = SystemColors.WindowTextBrush;
            }
            else
            {
                backLinkRectangle.Fill = SystemColors.GrayTextBrush;
            }
        }

        private bool doNavWorkaround;
    }
}