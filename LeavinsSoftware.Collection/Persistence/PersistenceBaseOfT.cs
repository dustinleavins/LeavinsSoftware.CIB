// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Base class for <see cref="IPersistence&lt;TModel&gt;"/> instances.
    /// </summary>
    /// <typeparam name="TModel">The type of data to persist.</typeparam>
    public abstract class PersistenceBase<TModel> : IPersistence<TModel> where TModel : Model
    {
        public TModel Create(TModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null");
            }
            else if (item.IsSummary)
            {
                throw new ArgumentException("cannot persist 'summary' item", "item");
            }
            else if (!item.IsNew)
            {
                throw new ArgumentException("cannot 'create' existing item", "item");
            }
            
            item.Validate();
            return CreateBase(item);
        }

        protected abstract TModel CreateBase(TModel item);

        public TModel Retrieve(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be positive", "id");
            }
            
            TModel item =  RetrieveBase(id);
            
            if (item != null && item.IsSummary)
            {
                throw new InvalidOperationException(
                    "RetrieveBase tried to return a 'summary' item");
            }
            
            return item;
        }

        protected abstract TModel RetrieveBase(long id);

        public TModel Update(TModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null");
            }
            else if (item.IsSummary)
            {
                throw new ArgumentException("cannot persist 'summary' item", "item");
            }
            else if (item.IsNew)
            {
                throw new ArgumentException("cannot update a new item", "item");
            }
            
            item.Validate();
            return UpdateBase(item);
        }

        protected abstract TModel UpdateBase(TModel item);

        public void Delete(TModel item)
        {
            DeleteBase(item);
        }

        protected abstract void DeleteBase(TModel item);
    }
}
