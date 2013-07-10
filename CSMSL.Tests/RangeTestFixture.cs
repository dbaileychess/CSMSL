using Moq;
using NUnit.Framework;

namespace CSMSL.Tests
{
    [TestFixture]
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
            var range1 = new Range<int>(3, 10);

            Assert.AreEqual(-1, range1.CompareTo(1));
        }

        [Test]
        public void RangeCompareToWithin()
        {
            var range1 = new Range<int>(3, 10);

            Assert.AreEqual(0, range1.CompareTo(5));
        }

        [Test]
        public void RangeCompareToAbove()
        {
            var range1 = new Range<int>(3, 10);

            Assert.AreEqual(1, range1.CompareTo(12));
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
            Assert.Throws<System.ArgumentException>(() => { _ignore = new Range<int>(10, 5); });
        }
    }
}
