// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.

using System;
using System.Globalization;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Program
{
    public static class AppKeys
    {
        /// <summary>
        /// Gets the key used for storing the last distribution type
        /// that the collector used for the category.
        /// </summary>
        /// <param name="primaryCategory"></param>
        /// <returns></returns>
        public static string VideoGame_LastDistributionType(ItemCategory primaryCategory)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "vg_{0}",
                primaryCategory.Name);
        }
    }
}
