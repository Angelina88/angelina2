using System.Collections.Generic;

namespace RingBuffer
{
    /// <summary>
    /// Буфер сегментов объектов T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuffer<T>
    {
        int Head { get; }
        int End { get;}

        IEnumerable<T[]> Pop(int count);
        int Push(IEnumerable<T[]> items);
    }
}

