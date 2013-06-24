// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;
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
            ItemCategory category = Persistence.CategoryPersistence.Retrieve(categoryId);
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

            if (model.Item.HasId)
            {
                Title = InterfaceResources.PageTitles_ItemExistingComic;
            }
            else
            {
                Title = InterfaceResources.PageTitles_ItemNewComic;
            }
        }
    }
}
