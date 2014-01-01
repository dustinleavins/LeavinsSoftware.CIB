using LeavinsSoftware.Collection.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Tests.Helpers
{
    public static class AssertEquality
    {
        public static void For(ItemCategory expected, ItemCategory actual)
        {
            string msg = string.Format("expected ID: {0}", expected.Id);
            
            Assert.AreEqual(expected.Id, actual.Id, msg);
            Assert.AreEqual(expected.Name, actual.Name, msg);
            Assert.AreEqual(expected.Code, actual.Code, msg);
            Assert.AreEqual(expected.CategoryType, actual.CategoryType, msg);
        }
        
        public static void For(ComicBookSeries expected, ComicBookSeries actual)
        {
            string msg = string.Format("expected ID: {0}", expected.Id);
            
            Assert.AreEqual(expected.Id, actual.Id, msg);
            Assert.AreEqual(expected.Name, actual.Name, msg);
            Assert.AreEqual(expected.Notes, actual.Notes, msg);
            Assert.AreEqual(expected.Publisher, actual.Publisher, msg);

            foreach (ComicBookSeriesEntry expectedEntry in expected.Entries)
            {
                string entryMsg = string.Format("expected ID: {0} entry ID: {1}",
                    expected.Id,
                    expectedEntry.Id);
                
                ComicBookSeriesEntry actualEntry = actual.Entries
                    .SingleOrDefault(e => e.Id == expectedEntry.Id);

                Assert.IsNotNull(actualEntry, entryMsg);
                For(expectedEntry, actualEntry, entryMsg);
            }
            
            For(expected.Publisher, actual.Publisher);
        }

        public static void For(ComicBookSeriesEntry expected, ComicBookSeriesEntry actual)
        {
            For(expected, actual, string.Format("expected ID: {0}", expected.Id));
        }

        public static void For(ComicBookSeriesEntry expected, ComicBookSeriesEntry actual, string msg)
        {
            Assert.AreEqual(expected.Id, actual.Id, msg);
            Assert.AreEqual(expected.SeriesId, actual.SeriesId, msg);
            Assert.AreEqual(expected.Name, actual.Name, msg);
            Assert.AreEqual(expected.Notes, actual.Notes, msg);
            Assert.AreEqual(expected.Number, actual.Number, msg);
            Assert.AreEqual(expected.ListType, actual.ListType, msg);
            Assert.AreEqual(expected.Condition, actual.Condition, msg);
            Assert.AreEqual(expected.Cover, actual.Cover, msg);
            Assert.AreEqual(expected.DistributionType, actual.DistributionType, msg);
            Assert.AreEqual(expected.EntryType, actual.EntryType, msg);
        }
        
        public static void For(Product expected, Product actual)
        {
            string msg = string.Format("expected ID: {0}", expected.Id);

            Assert.AreEqual(expected.Id, actual.Id, msg);
            Assert.AreEqual(expected.Name, actual.Name, msg);
            Assert.AreEqual(expected.Notes, actual.Notes, msg);
            Assert.AreEqual(expected.Quantity, actual.Quantity, msg);
            Assert.AreEqual(expected.ListType, actual.ListType, msg);
            
            For(expected.Category, actual.Category);
        }
        
        public static void For(VideoGame expected, VideoGame actual)
        {
            string msg = string.Format("expected ID: {0}", expected.Id);

            Assert.AreEqual(expected.Id, actual.Id, msg);
            Assert.AreEqual(expected.Name, actual.Name, msg);
            Assert.AreEqual(expected.Notes, actual.Notes, msg);
            Assert.AreEqual(expected.Condition, actual.Condition, msg);
            Assert.AreEqual(expected.ContentProvider, actual.ContentProvider, msg);
            Assert.AreEqual(expected.DistributionType, actual.DistributionType, msg);
            
            For(expected.System, actual.System);
        }
    }
}
