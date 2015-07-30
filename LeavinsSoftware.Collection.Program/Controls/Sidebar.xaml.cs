// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Controls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeavinsSoftware.Collection.Program.Controls
{
    /// <summary>
    /// Interaction logic for Sidebar.xaml
    /// </summary>
    public partial class Sidebar : UserControl
    {
        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register("Frame", typeof(Frame), typeof(Sidebar));

        public Sidebar()
        {
            InitializeComponent();
            var mainWindow = App.Current.MainWindow as MainWindow;
            DataContext = new SidebarViewModel(new AppNavImpl(this));
        }

        public Frame Frame
        {
            get
            {
                return GetValue(FrameProperty) as Frame;
            }
            set
            {
                SetValue(FrameProperty, value);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            if (Persistence.GetInstance<ICategoryPersistence>().Any(ItemCategoryType.Product))
            {
                productLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                productLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            // Comic Book
            if (Persistence.GetInstance<ICategoryPersistence>().Any(ItemCategoryType.ComicBook))
            {
                comicLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                comicLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            // Video Game
            if (Persistence.GetInstance<ICategoryPersistence>().Any(ItemCategoryType.VideoGame))
            {
                gameLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                gameLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private class AppNavImpl : IAppNavigationService
        {
            public AppNavImpl(Sidebar instance)
            {
                Instance = instance;
            }

            public Sidebar Instance
            {
                get;
                private set;
            }

            public Page CurrentPage
            {
                get
                {
                    return Instance.Frame.Content as Page;
                }
            }

            public void GoBack()
            {
                if (Instance.Frame.CanGoBack)
                {
                    Instance.Frame.GoBack();
                }
            }

            public void Navigate<TPage>(Func<TPage> page) where TPage : Page
            {
                Instance.Frame.Navigate(page());
            }
        }
    }
}
