// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Program.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace LeavinsSoftware.Collection.Program
{
    public abstract class CategoryPage : Page
    {
        public static CategoryPage PageFor(ItemCategoryType type)
        {
            switch (type)
            {
                case ItemCategoryType.ComicBook:
                    return new CategoryComicPage();
                case ItemCategoryType.Product:
                    return new CategoryProductPage();
                case ItemCategoryType.VideoGame:
                    return new CategoryGamePage();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
