// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using LeavinsSoftware.Collection.Models;
using LeavinsSoftware.Collection.Persistence;
using NUnit.Framework;

namespace LeavinsSoftware.Collection.Tests.Persistence
{
    [TestFixture]
    public sealed class ProgramOptionsPersistenceTests
    {
        private IProgramOptionsPersistence target;

        [SetUp]
        public void SetUp()
        {
            if (File.Exists("options.xml"))
            {
                File.Delete("options.xml");
            }
            
            target = new ProgramOptionsPersistence(new FileInfo("options.xml"));
        }

        [Test]
        public void PersistenceTest()
        {
            // Initially, options should be equal to defaults
            Assert.AreEqual((new ProgramOptions()), target.Retrieve());

            // Update test
            ProgramOptions newOptions = new ProgramOptions()
            {
                CheckForProgramUpdates = true,
                IsFirstRun = false,
                UseProxyServer = true,
                ProxyServerAddress = "10.10.10.1",
                ProxyServerPort = 81
            };

            target.Update(newOptions);
            Assert.AreEqual(newOptions, target.Retrieve());
        }
    }
}
