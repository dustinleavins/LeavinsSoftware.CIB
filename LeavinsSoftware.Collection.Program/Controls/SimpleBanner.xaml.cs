// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using KSMVVM.WPF;
using System;
using System.Windows;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program.Controls
{
    /// <summary>
    /// Interaction logic for SimpleBanner.xaml
    /// </summary>
    public partial class SimpleBanner : Banner
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SimpleBanner));

        public SimpleBanner()
        {
            Init();
        }

        public SimpleBanner(String text)
        {
            Init();
            SetValue(TextProperty, text);
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
