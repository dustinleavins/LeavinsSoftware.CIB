// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using System.Windows.Input;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class AddMainCategoryViewModel : ViewModelBase
    {
        public AddMainCategoryViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            NewProductCategory = new CustomCommand((unused) =>
                {
                    Nav.Navigate(() => new AddSubCategoryPage(ItemCategoryType.Product));
                });

            NewComicCategory = new CustomCommand((unused) =>
                {
                    Nav.Navigate(() => new AddSubCategoryPage(ItemCategoryType.ComicBook));
                });

            NewGameCategory = new CustomCommand((x) =>
                {
                    Nav.Navigate(() => new AddSubCategoryPage(ItemCategoryType.VideoGame));
                });
        }

        public ICommand NewProductCategory { get; private set; }

        public ICommand NewComicCategory { get; private set; }

        public ICommand NewGameCategory { get; private set; }

        public IAppNavigationService Nav { get; private set; }
    }
}
