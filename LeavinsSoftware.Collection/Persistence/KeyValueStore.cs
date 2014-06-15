// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using LeavinsSoftware.Collection.SQLite;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// <see cref="IKeyValueStore"/> implementation using SQLite.
    /// </summary>
    /// <remarks>
    /// Values are serialized as JSON.
    /// </remarks>
    public sealed class KeyValueStore : IKeyValueStore
    {
        public KeyValueStore(DirectoryInfo dataDir, Profile initialProfile)
        {
            string fullPath = Path.Combine(dataDir.FullName, initialProfile.Name, "collection.db");
            ConnectionString = string.Format(CultureInfo.InvariantCulture,
                "Data Source=|DataDirectory|{0}",
                fullPath);
        }
        
        public T GetValue<T>(int key)
        {
            bool setValue = false;
            T value = default(T);
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT v " +
                        "FROM KeyValues " +
                        "WHERE k = @key;";
                    
                    cmd.Parameters.Add(new SQLiteParameter("key", key));
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            setValue = true;
                            object v = reader["v"];
                            value = FromDb<T>(v.ToString());
                        }
                    }
                }
                
                connection.Close();
            }
            
            if (!setValue)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                return value;
            }
        }

        public void Save<T>(int key, T value)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                
                if (HasKey(key))
                {
                    SQL.Update("KeyValues")
                        .Set("v", ToDb(value))
                        .WhereEquals("k", key)
                        .ExecuteWith(connection);
                }
                else
                {
                    SQL.InsertInto("KeyValues")
                        .Column("k", key)
                        .Column("v", ToDb(value))
                        .ExecuteWith(connection);
                }
                
                connection.Close();
            }
        }

        public bool HasKey(int key)
        {
            bool hasKey = false;
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT 1 " +
                        "FROM KeyValues " +
                        "WHERE k = @key;";
                    
                    cmd.Parameters.Add(new SQLiteParameter("key", key));
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        hasKey = reader.Read();
                    }
                }
                
                connection.Close();
            }
            
            return hasKey;
        }
        
        public string ConnectionString
        {
            get;
            private set;
        }
        
        private string ToDb<T>(T dataValue)
        {
            return JsonConvert.SerializeObject(dataValue);
        }
        
        private T FromDb<T>(string stringValue)
        {
            return (T)JsonConvert.DeserializeObject(stringValue, typeof(T));
        }
    }
}
