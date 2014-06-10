﻿// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using System.Windows.Input;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="HomePage"/>.
    /// </summary>
    public sealed class HomeViewModel : ViewModelBase
    {
        public HomeViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            AddCategory = new CustomCommand((unused) =>
                {
                    Nav.Navigate(() => new AddMainCategoryPage());
                });

            ProductCategory = new CustomCommand((unused) =>
                {
                    Nav.Navigate(() => CategoryPage.PageFor<Product>());
                });

            ComicCategory = new CustomCommand((x) =>
                {
                    Nav.Navigate(() => CategoryPage.PageFor<ComicBookSeries>());
                });

            GameCategory = new CustomCommand((x) =>
            {
                Nav.Navigate(() => CategoryPage.PageFor<VideoGame>());
            });

            Export = new CustomCommand(
                (x) =>
                {
                    Nav.Navigate(() => new ExportPage());
                });

            Import = new CustomCommand(
                (x) =>
                {
                    Nav.Navigate(() => new ImportPage());
                });

            Options = new CustomCommand(
                (x) =>
                {
                    Nav.Navigate(() => new OptionsPage());
                });
        }

        public ICommand ProductCategory { get; private set; }

        public ICommand AddCategory { get; private set; }

        public ICommand ComicCategory { get; private set; }

        public ICommand GameCategory { get; private set; }

        public ICommand Export { get; private set; }

        public ICommand Import { get; private set; }

        public ICommand Options { get; private set; }

        public IAppNavigationService Nav { get; private set; }
    }
}
