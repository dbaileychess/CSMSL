// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CSMSL.Util.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Tests.Util.Collections
{
    [TestFixture, Category("Collections")]
    public sealed class SortedMaxSizedContainerTestFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Max size must be a positive, non-zero value\r\nParameter name: maxSize")]
        public void SortedMaxSizedContainerMaxSizeZero()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Max size must be a positive, non-zero value\r\nParameter name: maxSize")]
        public void SortedMaxSizedContainerMaxSizeNegative()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(-5);
        }

        [Test]
        public void SortedMaxSizedContainerMaxSize()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5);
            Assert.AreEqual(container.MaxSize, 5);
        }

        [Test]
        public void SortedMaxSizedContainerCountIntiallyZero()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5);
            Assert.AreEqual(container.Count, 0);
        }

        [Test]
        public void SortedMaxSizedContainerMaxSizeNotExceeded()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 1, 2, 3, 4, 5, 6, 7 };
            Assert.AreEqual(container.Count, container.MaxSize);
        }

        [Test]
        public void SortedMaxSizedContainerClear()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 1, 2, 3 };
            container.Clear();
            Assert.AreEqual(container.Count, 0);
        }

        [Test]
        public void SortedMaxSizedContainerClearMaxSizeUneffected()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 1, 2, 3 };
            container.Clear();
            Assert.AreEqual(container.MaxSize, 5);
        }

        [Test]
        public void SortedMaxSizedContainerOnlySmallestItemsRemain()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void SortedMaxSizedContainerContainsItem()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };

            Assert.IsTrue(container.Contains(3));
        }

        [Test]
        public void SortedMaxSizedContainerDoesNotContainsItem()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };

            Assert.IsFalse(container.Contains(7));
        }

        [Test]
        public void SortedMaxSizedContainerRemoveNonExistentItem()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };

            Assert.IsFalse(container.Remove(7));
        }

        [Test]
        public void SortedMaxSizedContainerRemoveExistentItem()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };

            Assert.IsTrue(container.Remove(3));
        }

        [Test]
        public void SortedMaxSizedContainerRemoveExistentItemDoesntContain()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            container.Remove(3);
            Assert.IsFalse(container.Contains(3));
        }

        [Test]
        public void SortedMaxSizedContainerRemoveExistentItemProperCount()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            container.Remove(3);
            Assert.AreEqual(container.Count, 4);
        }

        [Test]
        public void SortedMaxSizedContainerRemoveExistentItemCorrectCollection()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            container.Remove(3);
            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 1, 2, 4, 5 });
        }

        [Test]
        public void SortedMaxSizedContainerRemoveNonExistentItemCorrectCollection()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            container.Remove(8);
            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void SortedMaxSizedContainerRemoveLastItemCorrectCollection()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            container.Remove(5);
            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 1, 2, 3, 4 });
        }

        [Test]
        public void SortedMaxSizedContainerRemoveFirstItemCorrectCollection()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 7, 6, 5, 4, 3, 2, 1 };
            container.Remove(1);
            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 2, 3, 4, 5 });
        }

        [Test]
        public void SortedMaxSizedContainerRemoveDuplicateItemCorrectCollection()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 1, 1, 1, 1, 1, 1, 1 };
            container.Remove(1);
            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 1, 1, 1, 1 });
        }

        [Test]
        public void SortedMaxSizedContainerAddDefaultItem()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5) { 0, 0, 0, 0, 0, 0, 0, 0 };

            Assert.AreEqual(container.Count, 5);
        }

        [Test]
        public void SortedMaxSizedContainerInverseComparer()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(5, Comparer<int>.Create((a, b) => b.CompareTo(a))) { 7, 6, 5, 4, 3, 2, 1 };

            int[] data = container.ToArray();
            Assert.AreEqual(data, new[] { 7, 6, 5, 4, 3 });
        }

        [Test]
        public void SortedMaxSizedContainerLarge()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(25);
            for (int i = 0; i < 30; i++)
                container.Add(i);

            Assert.AreEqual(container.Count, 25);
        }

        [Test]
        public void SortedMaxSizedContainerLargeContains()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(25);
            for (int i = 0; i < 30; i++)
                container.Add(i);

            Assert.IsTrue(container.Contains(17));
        }

        [Test]
        public void SortedMaxSizedContainerLargeRemove()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(25);
            for (int i = 0; i < 30; i++)
                container.Add(i);

            Assert.IsTrue(container.Remove(17));
        }

        [Test]
        public void SortedMaxSizedContainerLargeRemoveCount()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(25);
            for (int i = 0; i < 30; i++)
                container.Add(i);
            container.Remove(17);
            Assert.AreEqual(container.Count, 24);
        }

        [Test]
        public void SortedMaxSizedContainerLargeRemoveEach()
        {
            SortedMaxSizedContainer<int> container = new SortedMaxSizedContainer<int>(25);
            for (int i = 0; i < 30; i++)
                container.Add(i);
            for (int i = 0; i < 25; i++)
                container.Remove(i);
            Assert.AreEqual(container.Count, 0);
        }
    }
}