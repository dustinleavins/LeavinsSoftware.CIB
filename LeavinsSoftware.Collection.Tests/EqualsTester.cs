// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace LeavinsSoftware.Collection.Tests
{
    public sealed class EqualsTester
    {
        private EqualsTester()
        {
        }

        public static EqualsTester New(object objectA, object objectB)
        {
            return new EqualsTester
            {
                ObjectA = objectA,
                ObjectB = objectB
            };
        }

        public EqualsTester Property(string propertyName)
        {
            PropertyName = propertyName;
            return this;
        }

        public EqualsTester LeftHandValue(object leftHandValue)
        {
            (ObjectA.GetType()).GetProperty(PropertyName).SetValue(ObjectA, leftHandValue, null);
            return this;
        }

        public EqualsTester RightHandValue(object rightHandValue)
        {
            (ObjectB.GetType()).GetProperty(PropertyName).SetValue(ObjectB, rightHandValue, null);
            return this;
        }

        public EqualsTester Expect(bool expected)
        {
            ExpectedResult = expected;
            return this;
        }

        public EqualsTester CheckHashCodes()
        {
            CheckHashes = true;
            return this;
        }

        public void Do()
        {
            Assert.AreEqual(ExpectedResult,
                            ObjectA.Equals(ObjectB),
                            "Property: " + PropertyName);
            Assert.AreEqual(ExpectedResult,
                            ObjectB.Equals(ObjectA),
                            "Property: " + PropertyName);

            if (CheckHashes)
            {
                Assert.AreEqual(ExpectedResult,
                                ObjectA.GetHashCode().Equals(ObjectB.GetHashCode()),
                                "Failed GetHashCode() check. " +
                                "Property: " + PropertyName);
            }
        }

        private bool CheckHashes { get; set; }
        private bool ExpectedResult { get; set; }
        private string PropertyName { get; set; }
        private object ObjectA { get; set; }
        private object ObjectB { get; set; }
    }
}

