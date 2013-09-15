// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    public abstract class CollectionPage : Page
    {
        public static CollectionPage PageFor(ItemCategoryType type)
        {
            switch (type)
            {
                case ItemCategoryType.ComicBook:
                    return new CollectionComicPage();
                case ItemCategoryType.Product:
                    return new CollectionProductPage();
                case ItemCategoryType.VideoGame:
                    return new CollectionGamePage();
                default:
                    throw new NotImplementedException();
            }
        }

        public static CollectionPage PageFor(ItemCategory category)
        {
            switch (category.CategoryType)
            {
                case ItemCategoryType.ComicBook:
                    return new CollectionComicPage(category);
                case ItemCategoryType.Product:
                    return new CollectionProductPage(category);
                case ItemCategoryType.VideoGame:
                    return new CollectionGamePage(category);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
