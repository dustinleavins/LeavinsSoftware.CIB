// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using LeavinsSoftware.Collection.Models;
using NUnit.Framework;

namespace LeavinsSoftware.Collection.Tests.Models
{
    [TestFixture]
    public class ItemTests
    {
        [TestCase("Apples", "Apples")]
        [TestCase("Apples\nApples", "Apples...")]
        public void NotesSummaryTest(string notes, string expectedSummary)
        {
            var target = new ItemBase()
            {
                Id = 0,
                Name = "Test Item",
                Notes = notes
            };
            
            Assert.AreEqual(expectedSummary, target.NotesSummary);
        }
        
        private class ItemBase : Item
        {
        }
    }
}
