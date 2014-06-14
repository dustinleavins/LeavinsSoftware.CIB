// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public static class DefaultCategories
    {
        public static IEnumerable<IDefaultCategory> ProductCategories
        {
            get
            {
                return new List<IDefaultCategory>()
                {
                    GenerateDefaultCategory(ItemCategoryType.Product, "P01")
                };
            }
        }

        public static IEnumerable<IDefaultCategory> ComicBookCategories
        {
            get
            {
                return new List<IDefaultCategory>()
                {
                    GenerateDefaultCategory(ItemCategoryType.ComicBook, "DC"),
                    GenerateDefaultCategory(ItemCategoryType.ComicBook, "Marvel"),
                    GenerateDefaultCategory(ItemCategoryType.ComicBook, "Vertigo")
                };
            }
        }

        public static IEnumerable<IDefaultCategory> VideoGameCategories
        {
            get
            {
                return new List<IDefaultCategory>()
                {
                    GenerateCompositeCategory(ItemCategoryType.VideoGame, "Microsoft",
                    new List<DefaultCategory>
                    {
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "Xbox"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "Xbox360"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "XboxOne")
                    }),

                    GenerateCompositeCategory(ItemCategoryType.VideoGame, "Sony",
                    new List<DefaultCategory>
                    {
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "PS1"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "PS2"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "PS3"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "PS4"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "PSP"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "Vita")
                    }),

                    GenerateCompositeCategory(ItemCategoryType.VideoGame, "Nintendo",
                    new List<DefaultCategory>
                    {
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "WiiU"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "3DS"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "Wii"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "DS"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "GB"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "GBA"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "NES"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "SNES"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "N64"),
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "GCN")
                    }),

                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "PC")
                };
            }
        }

        public static IEnumerable<IDefaultCategory> CategoriesFor(ItemCategoryType type)
        {
            switch (type)
            {
                case ItemCategoryType.Product:
                    return ProductCategories;
                case ItemCategoryType.ComicBook:
                    return ComicBookCategories;
                case ItemCategoryType.VideoGame:
                    return VideoGameCategories;
                default:
                    throw new NotImplementedException();
            }
        }

        private static CompositeCategory GenerateCompositeCategory(ItemCategoryType type,
            string lookupName,
            IEnumerable<DefaultCategory> categories)
        {
            return new CompositeCategory(categories)
            {
                CategoryType = type,
                Name = (string)Application.Current.FindResource("group_" + lookupName)
            };
        }

        private static DefaultCategory GenerateDefaultCategory(ItemCategoryType type, string code)
        {
            return new DefaultCategory()
            {
                CategoryType = type,
                Code = code,
                Name = (string)Application.Current.FindResource("dc_" + code)
            };
        }
    }
}
