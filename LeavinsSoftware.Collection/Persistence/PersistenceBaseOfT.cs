﻿// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    // TODO: Add change event notification
    public abstract class PersistenceBase<T> : IPersistence<T> where T : Model
    {
        public T Create(T item)
        {
            item.Validate();
            return CreateBase(item);
        }

        protected abstract T CreateBase(T item);

        public T Retrieve(long id)
        {
            return RetrieveBase(id);
        }

        protected abstract T RetrieveBase(long id);

        public T Update(T item)
        {
            item.Validate();
            return UpdateBase(item);
        }

        protected abstract T UpdateBase(T item);

        public void Delete(T item)
        {
            DeleteBase(item);
        }

        protected abstract void DeleteBase(T item);
    }
}