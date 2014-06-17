// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using LeavinsSoftware.Collection.Models;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class ItemCategoryPersistence : ICategoryPersistence
    {
        public ItemCategoryPersistence(DirectoryInfo dataDir, Profile initialProfile)
        {
            string fullPath = Path.Combine(dataDir.FullName, initialProfile.Name, "collection.db");
            ConnectionString = string.Format(CultureInfo.InvariantCulture,
                "Data Source=|DataDirectory|{0}",
                fullPath);
        }

        public string ConnectionString { get; private set; }

        public ItemCategory Create(ItemCategory item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null");
            }
            
            item.Validate();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.InsertInto("Categories")
                    .Column("name", item.Name)
                    .Column("type", item.CategoryType)
                    .Column("code", item.Code)
                    .ExecuteWith(connection);

                item.Id = connection.LastInsertRowId;
                connection.Close();
            }

            // TODO: Trigger changed event
            return item;
        }

        public ItemCategory Retrieve(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be positive", "id");
            }
            ItemCategory targetCategory = null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT categoryid, name, code, type " +
                        "FROM Categories " +
                        "WHERE rowid = @id";

                    cmd.Parameters.Add(new SQLiteParameter("id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            targetCategory = ReaderToCategory(reader);
                        }
                    }
                }

                connection.Close();
            }

            return targetCategory;
        }

        public ItemCategory Update(ItemCategory item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null");
            }
            
            item.Validate();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.Update("Categories")
                    .Set("name", item.Name)
                    .Set("type", item.CategoryType)
                    .Set("code", item.Code)
                    .WhereEquals("categoryid", item.Id)
                    .ExecuteWith(connection);

                connection.Close();
            }

            // TODO: Trigger changed event
            return item;
        }

        public void Delete(ItemCategory item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.DeleteFrom("Categories")
                    .WhereEquals("categoryid", item.Id)
                    .ExecuteWith(connection);

                connection.Close();
            }

            // TODO: Trigger changed event
        }

        public ICollection<ItemCategory> RetrieveAll(ItemCategoryType type)
        {
            List<ItemCategory> categories = new List<ItemCategory>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT categoryid, name, code, type " +
                        "FROM Categories " +
                        "WHERE type = @type";

                    int typeCode = (int)type;

                    cmd.Parameters.Add(new SQLiteParameter("type", typeCode.ToString(CultureInfo.InvariantCulture)));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(ReaderToCategory(reader));
                        }
                    }
                }

                connection.Close();
            }

            return categories;
        }

        public ICollection<ItemCategory> RetrieveAll()
        {
            List<ItemCategory> categories = new List<ItemCategory>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT categoryid, name, code, type " +
                        "FROM Categories;";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(ReaderToCategory(reader));
                        }
                    }
                }

                connection.Close();
            }

            return categories;
        }

        public long Count(ItemCategoryType type)
        {
            long count = 0;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) " +
                        "FROM Categories " +
                        "WHERE type = @type";

                    int typeCode = (int)type;

                    cmd.Parameters.Add(new SQLiteParameter("type", typeCode.ToString(CultureInfo.InvariantCulture)));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            long? countNullable = (long?)reader[0];

                            count = countNullable.GetValueOrDefault();
                        }
                    }
                }

                connection.Close();
            }

            return count;
        }

        public bool Any(ItemCategoryType type)
        {
            return Count(type) > 0;
        }

        private static ItemCategory ReaderToCategory(SQLiteDataReader reader)
        {
            var targetCategory = new ItemCategory();
            targetCategory.Id = long.Parse(reader["categoryid"].ToString(), CultureInfo.InvariantCulture);
            targetCategory.Name = reader["name"].ToString();
            targetCategory.Code = reader["code"].ToString();
            targetCategory.CategoryType = (ItemCategoryType)int.Parse(reader["type"].ToString(), CultureInfo.InvariantCulture);
            return targetCategory;
        }
    }
}
