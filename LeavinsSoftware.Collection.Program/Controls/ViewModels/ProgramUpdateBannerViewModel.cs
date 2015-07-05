// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Program.Controls.ViewInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.Controls.ViewModels
{
    public sealed class ProgramUpdateBannerViewModel : ViewModelBase
    {
        public IBannerView View
        {
            get;
            private set;
        }

        public ICommand Close
        {
            get;
            private set;
        }

        public ProgramUpdateBannerViewModel(IBannerView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view", "view cannot be null.");
            }

            View = view;

            Close = new CustomCommand(
                (_) =>
                {
                    View.Close();
                });
        }
    }
}
