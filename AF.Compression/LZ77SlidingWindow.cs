using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    internal class LZ77SlidingWindow : IEnumerable<LZ77SlidingWindow>
    {
        private static LZ77SlidingWindow _root;
        private LZ77SlidingWindow? _next;
        private int _count = 0;
        public byte Value { get; private set; }

        public LZ77SlidingWindow() { _root = this; }

        private LZ77SlidingWindow(byte value)
        {
            Value = value;
        }

        public LZ77SlidingWindow Push(byte value)
        {
            PushNext(value);
            Increase();
            Slide();
            return _root;
        }

        private void PushNext(byte value)
        {
            if (_root._count == 0)
                Value = value;
            else if (_next == null)
                _next = new LZ77SlidingWindow(value);
            else _next.PushNext(value);
        }
        private void Increase()
            => _root._count++;
        private void Slide()
        {
            if (_root._count > LZ77.WindowSize)
            {
                _root = _root._next;
                _root._count = LZ77.WindowSize;
            }
        }

        public IEnumerator<LZ77SlidingWindow> GetEnumerator()
        {
            yield return _root;
            LZ77SlidingWindow? next = _next;
            while (next != null)
            {
                yield return next;
                next = next._next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
