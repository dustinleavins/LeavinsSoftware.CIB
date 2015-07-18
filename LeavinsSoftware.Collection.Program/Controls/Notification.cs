// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Program.Controls.ViewInterfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LeavinsSoftware.Collection.Program.Controls
{
    public class Notification : UserControl, IBannerView
    {
        public static readonly DependencyProperty AutoHideProperty =
            DependencyProperty.Register("AutoHide", typeof(bool), typeof(Notification));

        private DispatcherTimer autoHideTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        public Notification()
        {
            Loaded += Banner_Loaded;
        }

        private void Banner_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoHide)
            {
                autoHideTimer.Tick += AutoHideTimer_Tick;
                autoHideTimer.Start();
            }
        }

        private void AutoHideTimer_Tick(object sender, EventArgs e)
        {
            RemoveFromParent();
            autoHideTimer.Stop();
        }

        public void Close()
        {
            RemoveFromParent();
        }

        public void RemoveFromParent()
        {
            var parent = Parent as Panel;

            if (parent == null)
            {
                throw new InvalidOperationException("Cannot remove this element from a parent whose class is not a Panel.");
            }

            parent.Children.Remove(this);
        }

        public bool AutoHide
        {
            get
            {
                return (bool)GetValue(AutoHideProperty);
            }
            set
            {
                SetValue(AutoHideProperty, value);
            }
        }
    }
}
