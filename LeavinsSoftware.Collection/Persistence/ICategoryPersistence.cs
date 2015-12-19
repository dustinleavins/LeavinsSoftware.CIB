// Copyright (c) 2013-2015 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Interface for <see cref="ItemCategory"/> persistence.
    /// </summary>
    public interface ICategoryPersistence : IPersistence<ItemCategory>
    {
        /// <summary>
        /// Retrieves all categories with the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ICollection<ItemCategory> RetrieveAll(ItemCategoryType type);

        /// <summary>
        /// Retrieves every category in persistence.
        /// </summary>
        /// <returns></returns>
        ICollection<ItemCategory> RetrieveAll();

        /// <summary>
        /// Retrieves the number of categories with the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        long Count(ItemCategoryType type);

        /// <summary>
        /// Are there any categories with the specified type?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Any(ItemCategoryType type);

        /// <summary>
        /// Are there any categories?
        /// </summary>
        /// <returns></returns>
        bool Any();

        event EventHandler<ModelAddedEventArgs<ItemCategory>> ItemAdded;
    }
}
