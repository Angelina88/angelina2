using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RingBuffer
{
    /// <summary>
    /// Кольцевой буфер сегментов объектов T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RingBuffer<T> : IBuffer<T>
    {
        private object syncObj = new object();
        private int _head;
        private int _end;

        private readonly int _size;
        private readonly int _itemSize;

        private volatile bool _full;

        private T[] _buffer;

        private object syncRoot = new object();

        public RingBuffer(int size, int itemSize)
        {
            if (size % itemSize != 0)
                throw new Exception("Неверные значения размера буфера и элемента");

            _size = size;
            _buffer = new T[_size];

            _itemSize = itemSize;

            _head = 0;
            _end = 0;
        }

        public int Head
        {
            get { return _head; }
            private set
            {
                lock (syncObj)
                {
                    _head = GetNewPosition(value);
                }
            }
        }

        public int End
        {
            get { return _end; }
            private set
            {
                lock (syncObj)
                {
                    _end = GetNewPosition(value);
                }
                
            }
        }

        /// <summary>
        /// Получение корректного указателя на элемент в буфере
        /// </summary>
        /// <param name="position">новая позиция</param>
        /// <returns>новая позиция, скорректированная установками буфера</returns>
        private int GetNewPosition(int position)
        {          
            if (position % _itemSize != 0)
                throw new ArgumentException("Значение указателя буфера заданно не верно");

            var res = position;
            while (res >= _size)
                res = res - _size;

            return res;
        }

        /// <summary>
        /// Получить список элементов
        /// </summary>
        /// <param name="count">количество элементов</param>
        /// <returns>список элементов с начала буфера</returns>
        public IEnumerable<T[]> Pop(int count)
        {           
            var list = new List<T[]>();
            for (var i = 0; i < count; i++)
            {
                var item = Pop();
                if (item != null)
                    list.Add(item);
                else break;
            };
            return list;
        }

        /// <summary>
        /// Получить следующий элемент
        /// </summary>
        /// <returns>первый элемент буфера или null, если буфер пуст</returns>
        private T[] Pop()
        {          
            if (Head == End && !_full) return null;

            Trace.WriteLine("Poping...");
            var item = new T[_itemSize];
            
            for(var i = 0; i < _itemSize; i++)
            {
                item[i] = _buffer[Head + i];
                Trace.TraceInformation("Poping item: {0}\n\r", item[i]);
            }
            Head += _itemSize;
            _full = false;
            return item;
        }

        /// <summary>
        /// Добавляет список элементов в буфер
        /// </summary>
        /// <param name="items">список элементов</param>
        /// <returns>количество успешно добавленных элементов</returns>
        public int Push(IEnumerable<T[]> items)
        {
            var i = 0;
            foreach (var item in items)
            {
                if (!Push(item)) break;
                i++;
            }
            return i;
        }

        /// <summary>
        /// Добавляет элемент в конец буфера
        /// </summary>
        /// <param name="item">элемент</param>
        /// <returns>признак успеха</returns>
        private bool Push(T[] item)
        {
            if (_full) return false;

            if (item.Length != _itemSize)
                throw new ArgumentException("Неверная величина элемента", "item");

            lock (syncRoot)
            {
                Trace.TraceInformation("Pushing item: {0}\n\r", item.ToString());
                for (var i = 0; i < item.Length; i++)
                {
                    _buffer[_end + i] = item[i];
                }
                End += _itemSize;
                if (End == Head) _full = true;
            }
            return true;
        }
    }
}