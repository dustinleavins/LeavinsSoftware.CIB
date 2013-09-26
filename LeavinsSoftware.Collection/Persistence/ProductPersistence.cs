// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.SQLite;
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class ProductPersistence :  PersistenceBase<Product>, IProductPersistence
    {
        public ProductPersistence(DirectoryInfo dataDir, Profile initialProfile)
        {
            string fullPath = Path.Combine(dataDir.FullName, initialProfile.Name, "collection.db");
            ConnectionString = string.Format(CultureInfo.InvariantCulture,
                "Data Source=|DataDirectory|{0}",
                fullPath);
        }

        public string ConnectionString { get; private set; }

        protected override Product CreateBase(Product item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.InsertInto("Products")
                    .Column("name", item.Name)
                    .Column("notes", item.Notes)
                    .Column("categoryid", item.Category.Id)
                    .Column("quantity", item.Quantity)
                    .Column("listtype", item.ListType)
                    .ExecuteWith(connection);

                item.Id = connection.LastInsertRowId;
                connection.Close();
            }

            return item;
        }

        protected override Product RetrieveBase(long id)
        {
            Product targetProduct = null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                long categoryId = 0;

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * " +
                        "FROM Products " +
                        "WHERE rowid = @id";

                    cmd.Parameters.Add(new SQLiteParameter("id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            targetProduct = ReaderToProduct(reader);

                            categoryId = long.Parse(reader["categoryid"].ToString(), CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (targetProduct != null)
                {
                    targetProduct.Category = GetCategory(categoryId, connection);
                }

                connection.Close();
            }

            return targetProduct;
        }

        protected override Product UpdateBase(Product item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.Update("Products")
                    .Set("name", item.Name)
                    .Set("notes", item.Notes)
                    .Set("categoryid", item.Category.Id)
                    .Set("quantity", item.Quantity)
                    .Set("listtype", item.ListType)
                    .WhereEquals("productid", item.Id)
                    .ExecuteWith(connection);

                connection.Close();
            }

            return item;
        }

        protected override void DeleteBase(Product item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                SQL.DeleteFrom("Products")
                    .WhereEquals("productid", item.Id)
                    .ExecuteWith(connection);

                connection.Close();
            }
        }

        public List<Product> Page(ModelSearchOptions options, long pageNumber)
        {
            List<Product> page = new List<Product>();

            Dictionary<long, long> categoryIdDict =
                new Dictionary<long, long>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * " +
                        "FROM Products " +
                        "WHERE name LIKE @name ";

                    string searchParamValue = "%";

                    if (!string.IsNullOrEmpty(options.SearchText))
                    {
                        searchParamValue = string.Format(CultureInfo.InvariantCulture,
                            "%{0}%", options.SearchText);
                    }

                    cmd.Parameters.Add(new SQLiteParameter("name", searchParamValue));

                    if (options.ItemCategory != null)
                    {
                        cmd.CommandText += "AND categoryid = @categoryid ";
                        cmd.Parameters.Add(new SQLiteParameter("@categoryid",
                            options.ItemCategory.Id));

                        if (options.ListType.HasValue)
                        {
                            cmd.CommandText += "AND listtype = @listtype ";
                            cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                        }
                    }
                    else if (options.ListType.HasValue)
                    {
                        cmd.CommandText += "AND listtype = @listtype ";
                        cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                    }

                    cmd.CommandText += "ORDER BY name LIMIT @limit OFFSET @offset;";
                    cmd.Parameters.Add(new SQLiteParameter("@limit", options.ItemsPerPage));
                    cmd.Parameters.Add(new SQLiteParameter("@offset", options.ItemsPerPage * pageNumber));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = ReaderToProduct(reader);

                            categoryIdDict[product.Id] =
                                long.Parse(reader["categoryid"].ToString(), CultureInfo.InvariantCulture);
                            
                            page.Add(product);
                        }
                    }
                }

                if (options.ItemCategory != null)
                {
                    foreach (Product item in page)
                    {
                        item.Category = options.ItemCategory;
                    }
                }
                else
                {
                    foreach (Product item in page)
                    {
                        long categoryId = categoryIdDict[item.Id];
                        item.Category = GetCategory(categoryId, connection);
                    }
                }

                connection.Close();
            }

            return page;
        }

        public long TotalResults(ModelSearchOptions options)
        {
            long total = 0;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) " +
                        "FROM Products " +
                        "WHERE name LIKE @name ";

                    string searchParamValue = "%";

                    if (!string.IsNullOrEmpty(options.SearchText))
                    {
                        searchParamValue = string.Format(CultureInfo.InvariantCulture,
                            "%{0}%", options.SearchText);
                    }

                    cmd.Parameters.Add(new SQLiteParameter("name", searchParamValue));

                    // WHERE
                    if (options.ItemCategory != null)
                    {
                        cmd.CommandText += "AND categoryid = @categoryid ";
                        cmd.Parameters.Add(new SQLiteParameter("@categoryid",
                            options.ItemCategory.Id));

                        if (options.ListType.HasValue)
                        {
                            cmd.CommandText += "AND listtype = @listtype";
                            cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                        }
                    }
                    else if (options.ListType.HasValue)
                    {
                        cmd.CommandText += "AND listtype = @listtype";
                        cmd.Parameters.Add(new SQLiteParameter("listtype", options.ListType));
                    }

                    cmd.CommandText += ";";


                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var countNullable = reader[0] as long?;
                            total = countNullable.Value;
                        }
                    }
                }

                connection.Close();
            }

            return total;
        }

        private static ItemCategory GetCategory(long id, SQLiteConnection openConnection)
        {
            ItemCategory targetCategory = null;

            using (var cmd = openConnection.CreateCommand())
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
                        targetCategory = new ItemCategory();
                        targetCategory.Id = long.Parse(reader["categoryid"].ToString(), CultureInfo.InvariantCulture);
                        targetCategory.Name = reader["name"].ToString();
                        targetCategory.Code = reader["code"].ToString();
                        targetCategory.CategoryType = (ItemCategoryType)int.Parse(reader["type"].ToString(), CultureInfo.InvariantCulture);
                    }
                }
            }

            return targetCategory;
        }

        private static Product ReaderToProduct(SQLiteDataReader reader)
        {
            Product targetProduct = new Product();

            targetProduct.Id = long.Parse(reader["productid"].ToString(), CultureInfo.InvariantCulture);
            targetProduct.Name = reader["name"].ToString();
            targetProduct.Notes = reader["notes"].ToString();
            targetProduct.Quantity = long.Parse(reader["quantity"].ToString(), CultureInfo.InvariantCulture);
            targetProduct.ListType = (ItemListType)int.Parse(reader["listtype"].ToString(), CultureInfo.InvariantCulture);

            return targetProduct;
        }
    }
}
