// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using System.Windows.Input;

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

            GetStartedCommand = new CustomCommand(
                (_) =>
                {
                    Nav.Navigate(() => new AddMainCategoryPage());
                });
        }

        public IAppNavigationService Nav { get; private set; }

        public ICommand GetStartedCommand
        {
            get;
            private set;
        }
    }
}
