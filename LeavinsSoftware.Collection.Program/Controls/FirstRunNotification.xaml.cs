// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Program.Controls.ViewModels;
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
    /// Interaction logic for FirstRunNotification.xaml
    /// </summary>
    public partial class FirstRunNotification : Notification
    {
        public FirstRunNotification()
        {
            InitializeComponent();
            DataContext = new FirstRunBannerViewModel(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.RemoveFromParent();
        }
    }
}
