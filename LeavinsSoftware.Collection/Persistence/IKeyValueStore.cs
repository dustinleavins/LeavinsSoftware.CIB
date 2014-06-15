// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Interface for key-value storage.
    /// </summary>
    public interface IKeyValueStore
    {
        /// <summary>
        /// Retrieves a value from storage
        /// </summary>
        /// <param name="key"></param>
        /// <exception
        /// <returns></returns>
        T Value<T>(int key);
        
        /// <summary>
        /// Saves a key-value pair.
        /// </summary>
        /// <remarks>
        /// Creates a new key-value pair if necessary.
        /// Overwrites existing key-value pair with the same key.
        /// </remarks>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Save<T>(int key, T value);
        
        /// <summary>
        /// Is the key present in persistence?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool HasKey(int key);
    }
}
