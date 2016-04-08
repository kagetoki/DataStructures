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
			var heap = GetFilledIntHeap(HeapType.Min);
			CheckHeapOrder(heap);
			heap.UpdateHeapType(HeapType.Max);
			CheckHeapOrder(heap);
		}

		private void CheckHeapOrder(Heap<int, string> heap)
		{
			var list = new List<KeyValuePair<int, string>>(heap.Count);
			while (heap.Count > 0)
			{
				list.Add(heap.Extract());
			}
			int last = list.First().Key;
			for (int i = 1; i < list.Count; i++)
			{
				Assert.IsTrue(heap.HeapType == HeapType.Max ? list[i].Key <= last : list[i].Key >= last);
				last = list[i].Key;
			}
			foreach (var p in list)
			{
				heap.Add(p.Key, p.Value);
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
