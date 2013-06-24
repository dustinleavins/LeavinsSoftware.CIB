// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LeavinsSoftware.Collection.Program
{
    public static class DefaultCategories
    {
        static DefaultCategories()
        {
            productCategories = new Lazy<List<ItemCategory>>(() =>
                {
                    var list = new List<ItemCategory>()
                    {
                        GetCategory(ItemCategoryType.Product, "P01")
                    };

                    return list;
                });

            comicCategories = new Lazy<List<ItemCategory>>(() =>
                {
                    return new List<ItemCategory>()
                    {
                        GetCategory(ItemCategoryType.ComicBook, "DC"),
                        GetCategory(ItemCategoryType.ComicBook, "Marvel"),
                        GetCategory(ItemCategoryType.ComicBook, "Vertigo")
                    };
                });

            gameCategories = new Lazy<List<ItemCategory>>(() =>
                {
                    return new List<ItemCategory>()
                    {
                        // Microsoft
                        GetCategory(ItemCategoryType.VideoGame, "Xbox360"),

                        // Sony
                        GetCategory(ItemCategoryType.VideoGame, "PS3"),
                        GetCategory(ItemCategoryType.VideoGame, "Vita"),

                        // Nintendo
                        GetCategory(ItemCategoryType.VideoGame, "WiiU"),
                        GetCategory(ItemCategoryType.VideoGame, "3DS"),
                        GetCategory(ItemCategoryType.VideoGame, "Wii"),
                        GetCategory(ItemCategoryType.VideoGame, "DS"),

                        // Other
                        GetCategory(ItemCategoryType.VideoGame, "PC")
                    };
                });
        }

        public static List<ItemCategory> ProductCategories
        {
            get
            {
                return productCategories.Value;
            }
        }

        public static List<ItemCategory> ComicCategories
        {
            get
            {
                return comicCategories.Value;
            }
        }

        public static List<ItemCategory> GameCategories
        {
            get
            {
                return gameCategories.Value;
            }
        }

        public static List<ItemCategory> CategoriesFor(ItemCategoryType type)
        {
            switch (type)
            {
                case ItemCategoryType.Product:
                    return ProductCategories;
                case ItemCategoryType.ComicBook:
                    return ComicCategories;
                case ItemCategoryType.VideoGame:
                    return GameCategories;
                default:
                    throw new NotImplementedException();
            }
        }

        private static ItemCategory GetCategory(ItemCategoryType type, string code)
        {
            return new ItemCategory()
            {
                CategoryType = type,
                Code = code,
                Name = (string)Application.Current.FindResource("dc_" + code)
            };
        }

        private static Lazy<List<ItemCategory>> productCategories;
        private static Lazy<List<ItemCategory>> comicCategories;
        private static Lazy<List<ItemCategory>> gameCategories;
    }
}
