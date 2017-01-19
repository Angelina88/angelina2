using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingBuffer;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace RingBufferTests
{
    /// <summary>
    /// Сводное описание для RingBufferFewTest
    /// </summary>
    [TestClass]
    public class RingBufferFewTest
    {
        private IBuffer<byte> _buffer;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _buffer = new RingBuffer<byte>(64, 8);
        }

        [TestMethod]
        public void AsyncPushTest()
        {
            var item1 = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            var item2 = new byte[] { 2, 2, 2, 2, 2, 2, 2, 2 };
            var item3 = new byte[] { 3, 3, 3, 3, 3, 3, 3, 3 };
            var task1 = Task.Factory.StartNew(
                () => {
                    Thread.Sleep(100);
                    _buffer.Push(new List<byte[]> { item1 });
                });
            var task2 = Task.Factory.StartNew(
                () => {
                    _buffer.Push(new List<byte[]> { item2 });
                    Thread.Sleep(100);
                    _buffer.Push(new List<byte[]> { item3 });
                });
            Task.WaitAll(task1, task2);

            var resultItems = _buffer.Pop(3);

            Assert.AreEqual(3, resultItems.Count());
        }

        [TestMethod]
        public void AsyncPushAndPopTest()
        {
            var item1 = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            var item2 = new byte[] { 2, 2, 2, 2, 2, 2, 2, 2 };
            var item3 = new byte[] { 3, 3, 3, 3, 3, 3, 3, 3 };
            var task1 = Task.Factory.StartNew(
                () => {
                    Thread.Sleep(100);
                    _buffer.Push(new List<byte[]> { item1 });
                    Thread.Sleep(1000);
                    _buffer.Pop(1);
                });
            var task2 = Task.Factory.StartNew(
                () => {
                    _buffer.Push(new List<byte[]> { item2 });
                    Thread.Sleep(100);
                    _buffer.Push(new List<byte[]> { item3 });
                    Thread.Sleep(500);
                    _buffer.Pop(2);
                });
            var task3 = Task.Factory.StartNew(
                () => {
                    Thread.Sleep(1000);
                    _buffer.Push(new List<byte[]> { item1 });
                    Thread.Sleep(100);
                    _buffer.Push(new List<byte[]> { item1 });
                    _buffer.Pop(2);
                });
            Task.WaitAll(task1, task2, task3);

            var resultItems = _buffer.Pop(3);

            Assert.AreEqual(0, resultItems.Count());
        }
    }
}
