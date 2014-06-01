// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Tests.Models
{
    [TestFixture]
    public sealed class ProgramOptionsTests
    {
        private ProgramOptions target;
        
        [SetUp]
        public void SetUp()
        {
            target = new ProgramOptions();
            PropertyChangedChecker.ListenTo(target);
        }
        
        [TestCase("IsFirstRun", false, false, true)]
        [TestCase("IsFirstRun", true, false, false)]
        [TestCase("IsFirstRun", true, true, true)]
        [TestCase("CheckForProgramUpdates", false, false, true)]
        [TestCase("CheckForProgramUpdates", true, false, false)]
        [TestCase("CheckForProgramUpdates", true, true, true)]
        [TestCase("UseProxyServer", false, false, true)]
        [TestCase("UseProxyServer", true, false, false)]
        [TestCase("UseProxyServer", true, true, true)]
        [TestCase("ProxyServerAddress", "", "", true)]
        [TestCase("ProxyServerAddress", "localhost", "localhost2", false)]
        [TestCase("ProxyServerPort", int.MaxValue, int.MaxValue, true)]
        [TestCase("ProxyServerPort", 80, 0, false)]
        public void EqualsTest<T>(string propertyName, T leftValue, T rightValue, bool expected)
        {
            ProgramOptions objectB =  new ProgramOptions();
            
            EqualsTester.New(target, objectB)
                .Property(propertyName)
                .LeftHandValue(leftValue)
                .RightHandValue(rightValue)
                .Expect(expected)
                .CheckHashCodes()
                .Do();
        }
        
        [Test]
        public void EqualsNullTest()
        {
            Assert.IsFalse(target.Equals((ProgramOptions)null));
        }
    }
}
