// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for ItemComicPage.xaml
    /// </summary>
    public partial class ItemComicPage : Page
    {
        private ItemComicViewModel model;

        private ItemComicPage()
        {
            InitializeComponent();
        }

        public static ItemComicPage NewComicPage()
        {
            ItemComicPage page = new ItemComicPage();

            page.Setup(new ItemComicViewModel(new PageNavigationService(page)));

            return page;
        }

        public static ItemComicPage NewComicPage(long categoryId)
        {
            ItemCategory category = Persistence.GetInstance<ICategoryPersistence>().Retrieve(categoryId);
            ItemComicPage page = new ItemComicPage();

            page.Setup(new ItemComicViewModel(new PageNavigationService(page),
                category));

            return page;
        }

        public static ItemComicPage ExistingComicPage(long comicId)
        {
            ItemComicPage page = new ItemComicPage();

            page.Setup(new ItemComicViewModel(new PageNavigationService(page),
                comicId));

            return page;
        }

        private void Setup(ItemComicViewModel context)
        {
            model = context;
            DataContext = model;

            if (model.Item.IsNew)
            {
            	Title = InterfaceResources.PageTitles_ItemNewComic;
                
            }
            else
            {
                Title = InterfaceResources.PageTitles_ItemExistingComic;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            model.Item.Entries.CollectionChanged += Issues_CollectionChanged;
            RefreshErrorLabels();
            
            if (model.Item.IsNew)
            {
            	nameTextBox.Focus();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            model.Item.Entries.CollectionChanged -= Issues_CollectionChanged;
        }

        private void Issues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshErrorLabels();
        }

        private void RefreshErrorLabels()
        {
            if (model.Item.Entries.Any())
            {
                issuesRequiredLabel.Visibility = Visibility.Collapsed;
            }
            else
            {
                issuesRequiredLabel.Visibility = Visibility.Visible;
            }
        }
    }
}
