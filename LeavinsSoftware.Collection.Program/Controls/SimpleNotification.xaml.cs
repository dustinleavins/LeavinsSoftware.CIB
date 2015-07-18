// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using System;
using System.Windows;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.Controls
{
    /// <summary>
    /// Interaction logic for SimpleNotification.xaml
    /// </summary>
    public partial class SimpleNotification : Notification
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SimpleNotification));

        public SimpleNotification()
        {
            Init();
        }

        private void Init()
        {
            InitializeComponent();
            DataContext = this;
            CloseCommand = new CustomCommand(
                (_) =>
                {
                    RemoveFromParent();
                });
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public ICommand CloseCommand
        {
            get;
            private set;
        }
    }
}
