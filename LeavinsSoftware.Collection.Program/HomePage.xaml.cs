// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.ViewModels;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomeViewModel(new PageNavigationService(this));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int presentCategories = 0;

            // Product
            if (Persistence.GetInstance<ICategoryPersistence>().Any(ItemCategoryType.Product))
            {
                productLabel.Visibility = System.Windows.Visibility.Visible;
                presentCategories += 1;
            }
            else
            {
                productLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            // Comic Book
            if (Persistence.GetInstance<ICategoryPersistence>().Any(ItemCategoryType.ComicBook))
            {
                comicLabel.Visibility = System.Windows.Visibility.Visible;
                presentCategories += 1;
            }
            else
            {
                comicLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            // Video Game
            if (Persistence.GetInstance<ICategoryPersistence>().Any(ItemCategoryType.VideoGame))
            {
                gameLabel.Visibility = System.Windows.Visibility.Visible;
                presentCategories += 1;
            }
            else
            {
                gameLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            // 'Add Category'
            if (presentCategories == 0)
            {
                // Show large label for Add Category
                addCategoryLabel.Visibility = System.Windows.Visibility.Collapsed;
                largeAddCategoryLabel.Visibility = System.Windows.Visibility.Visible;
                importExportGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (presentCategories < 3)
            {
                // Show small label for Add Category
                addCategoryLabel.Visibility = System.Windows.Visibility.Visible;
                largeAddCategoryLabel.Visibility = System.Windows.Visibility.Collapsed;
                importExportGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                // All categories added; do not show either label for Add Category
                addCategoryLabel.Visibility = System.Windows.Visibility.Collapsed;
                largeAddCategoryLabel.Visibility = System.Windows.Visibility.Collapsed;
                importExportGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
