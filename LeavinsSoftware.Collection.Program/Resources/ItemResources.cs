// Copyright (c) 2014, 2021 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using SimpleInjector;
using System;

namespace LeavinsSoftware.Collection.Program.Resources
{
    public static class ItemResources
    {
        public static IItemSpecificResources<TItem> Get<TItem>() where TItem : Item
        {
            return container.Value.GetInstance<IItemSpecificResources<TItem>>();
        }

        public static IItemSpecificResources Get(ItemCategoryType category)
        {
            switch (category)
            {
                case ItemCategoryType.ComicBook:
                    return container.Value.GetInstance<IItemSpecificResources<ComicBookSeries>>();
                case ItemCategoryType.Product:
                    return container.Value.GetInstance<IItemSpecificResources<Product>>();
                case ItemCategoryType.VideoGame:
                    return container.Value.GetInstance<IItemSpecificResources<VideoGame>>();
                default:
                    throw new NotImplementedException();
            }
        }

        private static Lazy<Container> container = new Lazy<Container>(
            () =>
            {
                var c = new Container();
                c.RegisterInstance<IItemSpecificResources<ComicBookSeries>>(new ItemSpecific.ComicBookResources());
                c.RegisterInstance<IItemSpecificResources<Product>>(new ItemSpecific.ProductResources());
                c.RegisterInstance<IItemSpecificResources<VideoGame>>(new ItemSpecific.VideoGameResources());
                return c;
            });
    }
}
