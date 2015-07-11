// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Attributes;
using LeavinsSoftware.Collection.Program.Controls;
using LeavinsSoftware.Collection.Program.Resources;
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
                .SingleOrDefault(o => o is CollectionPageAttribute)
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

                Page categoryPage = CategoryPage.PageFor(collectAttribute.CategoryType);
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

            var pageContent = e.Content as Page;
            
            if (pageContent != null)
            {
                headerLabel.Content = pageContent.Title;
            }

            if (mainFrame.CanGoBack)
            {
                backLinkRectangle.Fill = SystemColors.WindowTextBrush;
            }
            else
            {
                backLinkRectangle.Fill = SystemColors.GrayTextBrush;
            }
            
            viewer.ScrollToTop();
        }

        private void New_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            BasicMessenger.Default.Send(MessageIds.App_New);
        }
        
        void Finish_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            BasicMessenger.Default.Send(MessageIds.App_Finish);
        }
        
        private bool doNavWorkaround;

        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BasicMessenger.Default.Register(MessageIds.App_ImportSuccess,
                () => notificationsPanel.Children.Add(new SimpleBanner(InterfaceResources.Import_SuccessMessage)));

            BasicMessenger.Default.Register(MessageIds.App_ExportSuccess,
                () => notificationsPanel.Children.Add(new SimpleBanner(InterfaceResources.Export_SuccessMessage)));

            var options = Persistence.GetInstance<IProgramOptionsPersistence>().Retrieve();

            if (options.IsFirstRun)
            {
                notificationsPanel.Children.Add(new FirstRunBanner());
            }
            else if (options.CheckForProgramUpdates)
            {
                Version v = await Persistence.UpdateNotifier.GetServerVersionAsync();
                if (v != null &&
                    v.CompareTo(Persistence.UpdateNotifier.ClientVersion) > 0)
                {
                    notificationsPanel.Children.Add(new ProgramUpdateBanner());
                }
            }
        }
    }
}
