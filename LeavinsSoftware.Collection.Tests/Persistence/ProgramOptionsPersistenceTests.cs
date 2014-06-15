// Copyright (c) 2013, 2014 Dustin Leavins
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
        private const string TestDirectory = "options_test";

        /// <summary>
        /// Name of options file.
        /// </summary>
        /// <remarks>
        /// Added after discovering a bug where the program uses its own
        /// directory (as opposed to the user's directory) to store the
        /// options file.
        /// </remarks>
        private static readonly string FileName = Path.Combine(TestDirectory, "options.xml");
        
        private IProgramOptionsPersistence target;

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            else if (!Directory.Exists(TestDirectory))
            {
                Directory.CreateDirectory(TestDirectory);
            }
            
            target = new ProgramOptionsPersistence(new FileInfo(FileName));
        }

        [Test]
        public void PersistenceTest()
        {
            Assert.IsFalse(File.Exists(FileName),
                "options file should not exist before persistence test");
            
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
            
            Assert.IsTrue(File.Exists(FileName),
                "options file did not save to the right location");
        }
    }
}
