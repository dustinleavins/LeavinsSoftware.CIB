// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using KSMVVM.WPF.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeavinsSoftware.Collection.Program.Controls
{
    /// <summary>
    /// Interaction logic for WelcomeNotification.xaml
    /// </summary>
    public partial class WelcomeNotification : Notification
    {
        public WelcomeNotification()
        {
            InitializeComponent();
            DataContext = this;

            CloseCommand = new CustomCommand(
                (_) =>
                {
                    RemoveFromParent();
                });

            GetStartedCommand = new CustomCommand(
                (_) =>
                {
                    BasicMessenger.Default.Send(MessageIds.App_GettingStarted);
                    RemoveFromParent();
                });
        }

        public ICommand CloseCommand
        {
            get;
            private set;
        }

        public ICommand GetStartedCommand
        {
            get;
            private set;
        }
    }
}
