// Copyright (c) 2013, 2014, 2021 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
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
        
        [OneTimeSetUp]
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
            const string key = "Key";
            const string value = "value";
            Assert.IsFalse(target.HasKey(key));
            
            target.Save(key, value);
            Assert.IsTrue(target.HasKey(key));
            Assert.AreEqual(value, target.GetValue<string>(key));
        }
        
        [Test]
        public void ValueKeyNotFoundExceptionTest()
        {
            Assert.Catch(typeof(KeyNotFoundException), () => target.GetValue<string>("Not Found"));
        }
        
        [Test]
        public void GetValueOrDefaultTest()
        {
            Assert.AreEqual(-1, target.GetValueOrDefault("2", -1));
            target.Save("2", 5);
            Assert.AreEqual(5, target.GetValueOrDefault("2", -1));
        }
    }
}
