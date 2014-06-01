// Copyright (c) 2013, 2014 Dustin Leavins
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
            EventType = type;
        }

        public T OldData { get; private set; }

        public T NewData { get; private set; }

        public PersistenceEventType EventType { get; private set; }

        public static PersistenceEventArgs<T> CreateEventArgs(T newData)
        {
            return new PersistenceEventArgs<T>(PersistenceEventType.Create)
            {
                NewData = newData
            };
        }
    }
}
