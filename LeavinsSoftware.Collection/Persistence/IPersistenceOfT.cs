// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Persistence
{
    public interface IPersistence<T> where T : Model
    {
        T Create(T item);

        T Retrieve(long id);

        T Update(T item);

        void Delete(T item);
    }
}
