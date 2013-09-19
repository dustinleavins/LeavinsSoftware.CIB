using LeavinsSoftware.Collection.Persistence;
// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
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
                AsyncProgramSetup();
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

        private static void AsyncProgramSetup()
        {
            Task.Factory.StartNew(
                () =>
                {
                    var options = Persistence.GetInstance<IProgramOptionsPersistence>()
                        .Retrieve();

                    if (options.IsFirstRun)
                    {
                        var result = MessageBox.Show(
                            InterfaceResources.Startup_CheckUpdatesPrompt,
                            InterfaceResources.ProgramName,
                            MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            options.CheckForProgramUpdates = true;
                        }

                        options.IsFirstRun = false;
                        Persistence.GetInstance<IProgramOptionsPersistence>().Update(options);
                    }
                    else if (options.CheckForProgramUpdates)
                    {
                        Action<Version> versionDelegate = (v) =>
                        {
                            if (v != null &&
                                v.CompareTo(Persistence.UpdateNotifier.ClientVersion) > 0)
                            {
                                MessageBox.Show(
                                    InterfaceResources.Startup_UpdateAvailable,
                                    InterfaceResources.ProgramName);
                            }
                        };

                        Persistence.UpdateNotifier
                            .GetServerVersionAsync(versionDelegate);
                    }
                });
        }
    }
}
