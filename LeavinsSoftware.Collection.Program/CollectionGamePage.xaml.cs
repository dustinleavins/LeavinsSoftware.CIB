// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System.Globalization;
using System.Windows.Controls;
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.Attributes;
using LeavinsSoftware.Collection.Program.Resources;
using LeavinsSoftware.Collection.Program.ViewModels;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for CollectionGamePage.xaml
    /// </summary>
    [CollectionPage(ItemCategoryType.VideoGame)]
    public partial class CollectionGamePage : CollectionPage
    {
        public CollectionGamePage() : this(null)
        {
        }

        public CollectionGamePage(ItemCategory category)
        {
            InitializeComponent();
            model = new CollectionGameViewModel(new PageNavigationService(this), category);
            DataContext = model;

            if (model.SubCategory != null)
            {
                Title = string.Format(
                    CultureInfo.InvariantCulture,
                    InterfaceResources.PageTitles_CollectionFormat,
                    InterfaceResources.Common_VideoGames,
                    model.SubCategory.Name);
            }
        }
        
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            model.OnLoaded();
            BasicMessenger.Default.Register(MessageIds.App_New, () => model.AddItem.Execute(null));
        }
        
        private void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BasicMessenger.Default.Unregister(MessageIds.App_New);
        }
        
        private CollectionGameViewModel model;
    }
}
