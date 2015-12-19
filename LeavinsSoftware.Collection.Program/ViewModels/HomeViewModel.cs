// Copyright (c) 2013-2015 Dustin Leavins
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

            Export = new CustomCommand((x) =>
                Nav.Navigate(() => new ExportPage()));

            Import = new CustomCommand((x) =>
                Nav.Navigate(() => new ImportPage()));

            Options = new CustomCommand((x) =>
                Nav.Navigate(() => new OptionsPage()));
        }

        public ICommand Export { get; private set; }

        public ICommand Import { get; private set; }

        public ICommand Options { get; private set; }

        public IAppNavigationService Nav { get; private set; }
    }
}
