﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class PersistenceEventArgs<T> : EventArgs
    {
        private PersistenceEventArgs(PersistenceEventType type)
        {
            Type = type;
        }

        public T OldData { get; private set; }

        public T NewData { get; private set; }

        public PersistenceEventType Type { get; private set; }

        public static PersistenceEventArgs<T> CreateEventArgs(T newData)
        {
            return new PersistenceEventArgs<T>(PersistenceEventType.Create)
            {
                NewData = newData
            };
        }
    }
}
