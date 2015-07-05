// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Program.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LeavinsSoftware.Collection.Program
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application, IDisposable
    {
        private Mutex appMutex;

        public App()
        {
            appMutex = new Mutex(false, @"Local\" + "LeavinsCollection");

            if (!appMutex.WaitOne(0, false))
            {
                MessageBox.Show("Already running");
                Application.Current.Shutdown();
            }
            else
            {
                Persistence.Setup();
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
            if (appMutex != null)
            {
                appMutex.Dispose();
                appMutex = null;
            }
        }
    }
}
