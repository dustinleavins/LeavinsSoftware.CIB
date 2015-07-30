// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.Controls.ViewModels
{
    public sealed class SidebarViewModel : ViewModelBase
    {
        public SidebarViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            ProductCategory = new CustomCommand((unused) =>
                Nav.Navigate(() => CategoryPage.PageFor<Product>()));

            ComicCategory = new CustomCommand((x) =>
                Nav.Navigate(() => CategoryPage.PageFor<ComicBookSeries>()));

            GameCategory = new CustomCommand((x) =>
                Nav.Navigate(() => CategoryPage.PageFor<VideoGame>()));
        }

        public IAppNavigationService Nav
        {
            get;
            private set;
        }

        public ICommand ProductCategory { get; private set; }

        public ICommand ComicCategory { get; private set; }

        public ICommand GameCategory { get; private set; }
    }
}
