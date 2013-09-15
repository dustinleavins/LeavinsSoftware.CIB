using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LeavinsSoftware.Collection.Program.Categories
{
    public sealed class DefaultCategories
    {
        public static IEnumerable<CategoryBase> ProductCategories
        {
            get
            {
                return new List<CategoryBase>()
                {
                    GenerateDefaultCategory(ItemCategoryType.Product, "P01")
                };
            }
        }

        public static IEnumerable<CategoryBase> ComicBookCategories
        {
            get
            {
                return new List<CategoryBase>()
                {
                    GenerateDefaultCategory(ItemCategoryType.ComicBook, "DC"),
                    GenerateDefaultCategory(ItemCategoryType.ComicBook, "Marvel"),
                    GenerateDefaultCategory(ItemCategoryType.ComicBook, "Vertigo")
                };
            }
        }

        public static IEnumerable<CategoryBase> VideoGameCategories
        {
            get
            {
                return new List<CategoryBase>()
                {
                    // Microsoft
                    GenerateCompositeCategory(ItemCategoryType.VideoGame, "Microsoft",
                    new List<DefaultCategory>
                    {
                        GenerateDefaultCategory(ItemCategoryType.VideoGame, "Xbox360"),
                    }),

                    // Sony
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "PS3"),
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "Vita"),

                    // Nintendo
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "WiiU"),
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "3DS"),
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "Wii"),
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "DS"),

                    // Other
                    GenerateDefaultCategory(ItemCategoryType.VideoGame, "PC")
                };
            }
        }

        public static IEnumerable<CategoryBase> CategoriesFor(ItemCategoryType type)
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
