// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Windows.Input;

namespace LeavinsSoftware.Collection.Program
{
    public static class AppCommands
    {
        private static readonly RoutedUICommand finish = new RoutedUICommand();
        
        public static RoutedUICommand Finish
        {
            get
            {
                return finish;
            }
        }
    }
}
