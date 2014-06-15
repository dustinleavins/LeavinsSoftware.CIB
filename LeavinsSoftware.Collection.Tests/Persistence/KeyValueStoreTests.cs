// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.IO;
using LeavinsSoftware.Collection.Persistence;
using LeavinsSoftware.Collection.Persistence.Migrations;
using NUnit.Framework;

namespace LeavinsSoftware.Collection.Tests.Persistence
{

    [TestFixture]
    public class KeyValueStoreTests
    {
        private IKeyValueStore target;
        
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            if (File.Exists(Path.Combine("kv", "collection.db")))
            {
                File.Delete(Path.Combine("kv", "collection.db"));
            }

            DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            Profile kvProfile = new Profile("kv");
            MigrationRunner.Run(currentDir, kvProfile);

            target = new KeyValueStore(currentDir, kvProfile);
        }
        
        [Test]
        public void PersistenceTest()
        {
            const int key = 0;
            const string value = "value";
            Assert.IsFalse(target.HasKey(key));
            
            target.Save(key, value);
            Assert.IsTrue(target.HasKey(key));
            Assert.AreEqual(value, target.GetValue<string>(key));
        }
        
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ValueKeyNotFoundExceptionTest()
        {
            target.GetValue<string>(-1);
        }
        
        [Test]
        public void GetValueOrDefaultTest()
        {
            Assert.AreEqual(-1, target.GetValueOrDefault(2, -1));
            target.Save(2, 5);
            Assert.AreEqual(5, target.GetValueOrDefault(2, -1));
        }
    }
}
