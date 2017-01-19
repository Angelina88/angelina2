using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingBuffer;
using System.Collections.Generic;
using System.Linq;

namespace RingBufferTests
{
    [TestClass]
    public class RingBufferSingleTest
    {
        [TestMethod]
        public void PushTest()
        {
            var buffer = new RingBuffer<byte>(32, 8);

            var count = buffer.Push(new List<byte[]>() { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 } });

            Assert.AreEqual(0, buffer.Head);
            Assert.AreEqual(8, buffer.End);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void PopTest()
        {
            var buffer = new RingBuffer<byte>(32, 8);
            var item = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            buffer.Push(new List<byte[]>() { item });

            var popItems = buffer.Pop(1);

            Assert.AreEqual(8, buffer.Head);
            Assert.AreEqual(8, buffer.End);
            Assert.AreEqual(1, popItems.Count());
            for (var i = 0; i < item.Length; i++)
                Assert.AreEqual(item[i], popItems.First()[i]);
        }

        [TestMethod]
        public void PushOverflowTest()
        {
            var buffer = new RingBuffer<byte>(32, 8);

            var count = buffer.Push(new List<byte[]>()
                                    {
                                        new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                                        new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                                        new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                                        new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                                        new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }
                                    });

            Assert.AreEqual(0, buffer.Head);
            Assert.AreEqual(0, buffer.End);
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void PopEmptyTest()
        {
            var buffer = new RingBuffer<byte>(32, 8);
            buffer.Push(new List<byte[]>());

            var popItems = buffer.Pop(1);

            Assert.AreEqual(0, buffer.Head);
            Assert.AreEqual(0, buffer.End);
            Assert.AreEqual(0, popItems.Count());

        }

        [TestMethod]
        public void PushAndPopTest()
        {
            var buffer = new RingBuffer<byte>(32, 8);

            var item = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var count = buffer.Push(new List<byte[]>() { item, item, item, item });

            var popItems = buffer.Pop(2);

            //Пересечение элеменами границы 0
            buffer.Push(new[] { item });

            Assert.AreEqual(16, buffer.Head);
            Assert.AreEqual(8, buffer.End);

            //Полное заполнение буфера на отметке отлично от 0
            count = buffer.Push(new[] { item, item });

            Assert.AreEqual(1, count);
            Assert.AreEqual(16, buffer.End);

            //Полное извлечение из буфера
            buffer.Pop(4);
            Assert.AreEqual(16, buffer.Head);
            Assert.AreEqual(16, buffer.End);

            //Перезаполнение буфера
            count = buffer.Push(new[] { item, item });

            Assert.AreEqual(2, count);
            Assert.AreEqual(0, buffer.End);

            buffer.Pop(2);
            Assert.AreEqual(0, buffer.Head);
        }
    }
}
