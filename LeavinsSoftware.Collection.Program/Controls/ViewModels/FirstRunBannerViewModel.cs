// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.ViewModel;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Controls.ViewInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.Controls.ViewModels
{
    public sealed class FirstRunBannerViewModel : ViewModelBase
    {
        public ICommand Accept
        {
            get;
            private set;
        }

        public ICommand Reject
        {
            get;
            private set;
        }

        public IBannerView View
        {
            get;
            private set;
        }

        public FirstRunBannerViewModel(IBannerView view)
        {
            View = view;

            Accept = new CustomCommand(
                (_) =>
                {
                    var options = Persistence.GetInstance<IProgramOptionsPersistence>().Retrieve();
                    options.IsFirstRun = false;
                    options.CheckForProgramUpdates = true;
                    Persistence.GetInstance<IProgramOptionsPersistence>().Update(options);
                    View.Close();
                });

            Reject = new CustomCommand(
                (_) =>
                {
                    var options = Persistence.GetInstance<IProgramOptionsPersistence>().Retrieve();
                    options.IsFirstRun = false;
                    options.CheckForProgramUpdates = false;
                    Persistence.GetInstance<IProgramOptionsPersistence>().Update(options);
                    View.Close();
                });
        }
    }
}
