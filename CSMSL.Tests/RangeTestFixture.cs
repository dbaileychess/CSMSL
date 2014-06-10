// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (RangeTestFixture.cs) is part of CSMSL.Tests.
//
// CSMSL.Tests is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CSMSL.Tests is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL.Tests. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using System;

namespace CSMSL.Tests
{
    [TestFixture, Category("Range")]
    public sealed class RangeTestFixture
    {
        // ReSharper disable NotAccessedField.Local
        private object _ignore;

        // ReSharper restore NotAccessedField.Local

        [Test]
        public void RangeSubRange()
        {
            var a = new Range<int>(5, 7);
            var b = new Range<int>(0, 10);

            Assert.IsTrue(a.IsSubRange(b));
        }

        [Test]
        public void RangeSubRangeReverseIsFalse()
        {
            var a = new Range<int>(5, 7);
            var b = new Range<int>(0, 10);

            Assert.IsFalse(b.IsSubRange(a));
        }

        [Test]
        public void RangeOverlappingIsFalse()
        {
            var range1 = new Range<int>(5, 10);
            var range2 = new Range<int>(15, 20);

            Assert.IsFalse(range1.IsOverlapping(range2));
        }

        [Test]
        public void RangeOverlappingIsFalseReverse()
        {
            var range1 = new Range<int>(5, 10);
            var range2 = new Range<int>(15, 20);

            Assert.IsFalse(range2.IsOverlapping(range1));
        }

        [Test]
        public void RangeOverlappingIsFalseWhenNull()
        {
            var range1 = new Range<int>(5, 10);

            Assert.IsFalse(range1.IsOverlapping(null));
        }

        [Test]
        public void RangeOverlappingIsTrue()
        {
            var range1 = new Range<int>(5, 10);
            var range2 = new Range<int>(7, 12);

            Assert.IsTrue(range1.IsOverlapping(range2));
        }

        [Test]
        public void RangeOverlappingIsTrueReverse()
        {
            var range1 = new Range<int>(5, 10);
            var range2 = new Range<int>(7, 12);

            Assert.IsTrue(range2.IsOverlapping(range1));
        }

        [Test]
        public void RangeOverlappingIsTrueLarger()
        {
            var range1 = new Range<int>(0, 10);
            var range2 = new Range<int>(3, 7);

            Assert.IsTrue(range1.IsOverlapping(range2));
        }

        [Test]
        public void RangeOverlappingIsTrueSmaller()
        {
            var range1 = new Range<int>(0, 10);
            var range2 = new Range<int>(3, 7);

            Assert.IsTrue(range2.IsOverlapping(range1));
        }

        [Test]
        public void RangeOverlappingIsTrueItSelf()
        {
            var range1 = new Range<int>(0, 10);

            Assert.IsTrue(range1.IsOverlapping(range1));
        }

        [Test]
        public void RangeDoesContainItem()
        {
            var range1 = new Range<int>(3, 10);

            Assert.IsTrue(range1.Contains(5));
        }

        [Test]
        public void RangeDoesnotContainItemHigher()
        {
            var range1 = new Range<int>(3, 10);

            Assert.IsFalse(range1.Contains(12));
        }

        [Test]
        public void RangeDoesnotContainItemLower()
        {
            var range1 = new Range<int>(3, 10);

            Assert.IsFalse(range1.Contains(1));
        }

        [Test]
        public void RangeDoesContainItemLowerBounds()
        {
            var range1 = new Range<int>(3, 10);

            Assert.IsTrue(range1.Contains(3));
        }

        [Test]
        public void RangeDoesContainItemUpperBounds()
        {
            var range1 = new Range<int>(3, 10);

            Assert.IsTrue(range1.Contains(10));
        }

        [Test]
        public void RangeCompareToBelow()
        {
            var range = new Range<int>(3, 10);
            int value = 1;

            int comp = range.CompareTo(value);

            Assert.AreEqual(-1, comp);
        }

        [Test]
        public void RangeCompareToWithin()
        {
            var range = new Range<int>(3, 10);
            int value = 5;

            int comp = range.CompareTo(value);

            Assert.AreEqual(0, comp);
        }

        [Test]
        public void RangeCompareToAbove()
        {
            var range = new Range<int>(3, 10);
            int value = 12;

            int comp = range.CompareTo(value);

            Assert.AreEqual(1, comp);
        }

        [Test]
        public void RangesAreEquivalent()
        {
            var range1 = new Range<int>(3, 10);
            var range2 = new Range<int>(3, 10);

            Assert.AreEqual(range1, range2);
        }

        [Test]
        public void RangesAreEquivalentNotReference()
        {
            var range1 = new Range<int>(3, 10);
            var range2 = new Range<int>(3, 10);

            Assert.AreNotSame(range1, range2);
        }

        [Test]
        public void RangeMinBiggerThanMax()
        {
            Assert.Throws<ArgumentException>(() => { _ignore = new Range<int>(10, 5); });
        }

        [Test]
        public void MassRangeFromDAWidth()
        {
            var range1 = new DoubleRange(10, new Tolerance(ToleranceUnit.DA, 4));

            Assert.AreEqual(8, range1.Width);
        }

        [Test]
        public void MassRangeFromDAMean()
        {
            var range1 = new DoubleRange(10, new Tolerance(ToleranceUnit.DA, 4));

            Assert.AreEqual(10, range1.Mean);
        }

        [Test]
        public void MassRangeFromDAMin()
        {
            var range1 = new DoubleRange(10, new Tolerance(ToleranceUnit.DA, 4));

            Assert.AreEqual(6, range1.Minimum);
        }

        [Test]
        public void MassRangeFromDAMax()
        {
            var range1 = new DoubleRange(10, new Tolerance(ToleranceUnit.DA, 4));

            Assert.AreEqual(14, range1.Maximum);
        }

        [Test]
        public void MassRangeFromDANullMean()
        {
            var range1 = new DoubleRange(10, null);

            Assert.AreEqual(10, range1.Mean);
        }

        [Test]
        public void MassRangeFromDANullWidth()
        {
            var range1 = new DoubleRange(10, null);

            Assert.AreEqual(0, range1.Width);
        }

        [Test]
        public void MassRangeFromDANullMin()
        {
            var range1 = new DoubleRange(10, null);

            Assert.AreEqual(10, range1.Minimum);
        }

        [Test]
        public void MassRangeFromDANullMax()
        {
            var range1 = new DoubleRange(10, null);

            Assert.AreEqual(10, range1.Maximum);
        }

        [Test]
        public void MassRangeFromDANegative()
        {
            var range1 = new DoubleRange(10, new Tolerance(ToleranceUnit.DA, 4));
            var range2 = new DoubleRange(10, new Tolerance(ToleranceUnit.DA, -4));

            Assert.AreEqual(range1, range2);
        }
    }
}