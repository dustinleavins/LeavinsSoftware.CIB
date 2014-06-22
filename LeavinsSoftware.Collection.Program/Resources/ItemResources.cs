using LeavinsSoftware.Collection.Models;
using ItemSpecific = LeavinsSoftware.Collection.Program.Resources.ItemSpecific;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                c.RegisterSingle<IItemSpecificResources<ComicBookSeries>>(new ItemSpecific.ComicBookResources());
                c.RegisterSingle<IItemSpecificResources<Product>>(new ItemSpecific.ProductResources());
                c.RegisterSingle<IItemSpecificResources<VideoGame>>(new ItemSpecific.VideoGameResources());
                return c;
            });
    }
}
