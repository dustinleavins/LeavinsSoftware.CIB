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

        private SidebarViewModel model;

        public Sidebar()
        {
            InitializeComponent();

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
                var pageInstance = page();
                bool removePreviousEntry = Instance.Frame.Content is CollectionPage && pageInstance is CollectionPage;

                if (removePreviousEntry)
                {
                    Instance.Frame.Navigated += Frame_Navigated;
                }

                Instance.Frame.Navigate(pageInstance);
            }

            private void Frame_Navigated(object sender, NavigationEventArgs e)
            {
                Instance.Frame.Navigated -= Frame_Navigated;
                Instance.Frame.RemoveBackEntry();
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                model = await SidebarViewModel.Create(new AppNavImpl(this));
                DataContext = model;
                Persistence.GetInstance<ICategoryPersistence>().ItemAdded += Sidebar_ItemAdded;
            }
        }

        private async void Sidebar_ItemAdded(object sender, ModelAddedEventArgs<ItemCategory> e)
        {
            if (Dispatcher.CheckAccess())
            {
                await model.UpdateSubCategories();
            }
            else
            {
                Action<Sidebar> d = async (instance) =>
                {
                    await instance.model.UpdateSubCategories();
                };
                await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    d,
                    this);
            }
        }
    }
}
