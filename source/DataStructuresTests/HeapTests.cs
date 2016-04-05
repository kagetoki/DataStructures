using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace DataStructuresTests
{
    [TestClass]
    public class HeapTests
    {
        [TestMethod]
        public void Heap_OrderItems()
        {
            var minHeap = GetFilledIntHeap(HeapType.Min);
            var minList = new List<int>();
            while (minHeap.Count > 0)
            {
                var item = minHeap.Extract();
                minList.Add(item.Key);
            }
            int minLast = minList.First();
            for (int i = 1; i < minList.Count; i++)
            {
                Assert.IsTrue(minList[i] >= minLast);
                minLast = minList[i];
            }

            var maxHeap = GetFilledIntHeap(HeapType.Max);
            var maxList = new List<int>();
            while (maxHeap.Count > 0)
            {
                var item = maxHeap.Extract();
                maxList.Add(item.Key);
            }
            int maxLast = maxList.First();
            for (int i = 1; i < maxList.Count; i++)
            {
                Assert.IsTrue(maxList[i] <= maxLast);
                maxLast = maxList[i];
            }
        }

        [TestMethod]
        public void Heap_EnumerateHeap()
        {
            var minHeap = GetFilledIntHeap();
            foreach (var item in minHeap)
            {
                Console.WriteLine("Key:{0}, Value:{1}", item.Key, item.Value);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Heap_ThrowOnChange()
        {
            var minHeap = GetFilledIntHeap();
            foreach (var item in minHeap)
            {
                minHeap.Add(2, "asdas");
            }
        }

        [TestMethod]
        public void Heap_GetByKey()
        {
            var heap = GetFilledIntHeap(HeapType.Max);
            int key = 20;
            var searchResult = heap.FindByKey(key);
            Assert.IsNotNull(searchResult);
            int entryCount = 0;
            foreach (var entry in heap)
            {
                if (entry.Key == key)
                {
                    entryCount++;
                }
            }

            Assert.IsTrue(searchResult.Count == entryCount);
        }

        private Heap<int, string> GetFilledIntHeap(HeapType heapType = HeapType.Min)
        {
            var heap = new Heap<int, string>(heapType);
            for (int i = 100; i > 0; i--)
            {
                heap.Add(i, i.ToString());
                heap.Add(i / 2, i.ToString());
            }
            return heap;
        }

        private Heap<string, string> GetFilledStringHeap(HeapType heapType = HeapType.Min)
        {
            var heap = new Heap<string, string>(heapType);
            for (int i = 100; i > 0; i--)
            {
                heap.Add("abc" + i, i.ToString());
                heap.Add("as" + (i / 2), i.ToString());
            }
            return heap;
        }
    }
}
