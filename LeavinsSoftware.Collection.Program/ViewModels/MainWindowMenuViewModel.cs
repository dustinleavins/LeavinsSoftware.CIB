using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.ViewModels
{
    public sealed class MainWindowMenuViewModel : ViewModelBase
    {
        public MainWindowMenuViewModel(IAppNavigationService nav)
        {
            Nav = nav;

            OptionsCommand = new CustomCommand(
                (_) =>
                {
                    Nav.Navigate(() => new OptionsPage());
                });

            ImportCommand = new CustomCommand(
                (_) =>
                {
                    Nav.Navigate(() => new ImportPage());
                });

            ExportCommand = new CustomCommand(
                (_) =>
                {
                    Nav.Navigate(() => new ExportPage());
                });
        }

        public IAppNavigationService Nav
        {
            get;
            private set;
        }

        public ICommand OptionsCommand
        {
            get;
            private set;
        }

        public ICommand ImportCommand
        {
            get;
            private set;
        }

        public ICommand ExportCommand
        {
            get;
            private set;
        }
    }
}
