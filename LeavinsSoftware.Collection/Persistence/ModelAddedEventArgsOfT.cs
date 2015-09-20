// Copyright (c) 2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class ModelAddedEventArgs<T> : EventArgs
        where T : Model
    {
        public ModelAddedEventArgs(T item)
        {
            ItemAdded = item;
        }

        public T ItemAdded
        {
            get;
            private set;
        }
    }
}
