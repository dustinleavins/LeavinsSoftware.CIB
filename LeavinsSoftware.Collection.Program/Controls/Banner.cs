// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Program.Controls.ViewInterfaces;
using System;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program.Controls
{
    public abstract class Banner : UserControl, IBannerView
    {
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
    }
}
