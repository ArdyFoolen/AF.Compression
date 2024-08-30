using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    internal class LZ77Pointer
    {
        private byte _offset;
        private byte _length;

        public LZ77Pointer(byte offset, byte length)
        {
            _offset = offset;
            _length = length;
        }
        //public LZ77Pointer(byte value)
        //{
        //    _offset = value >> 3;
        //    _length = value & 0x07;
        //}

        //public byte Value { get => (byte)((_offset << 3) | _length); }
        public byte Offset { get => _offset; }
        public byte Length { get => _length; }

        public static bool operator ==(LZ77Pointer left, LZ77Pointer right)
            => left._length == right._length;
        public static bool operator ==(LZ77Pointer left, int length)
            => left._length == length;

        public static bool operator >(LZ77Pointer left, LZ77Pointer right)
            => left._length > right._length;
        public static bool operator >(LZ77Pointer left, int length)
            => left._length > length;

        public static bool operator >=(LZ77Pointer left, LZ77Pointer right)
            => left._length >= right._length;
        public static bool operator >=(LZ77Pointer left, int length)
            => left._length >= length;

        public static bool operator !=(LZ77Pointer left, LZ77Pointer right)
            => left._length != right._length;
        public static bool operator !=(LZ77Pointer left, int length)
            => left._length != length;

        public static bool operator <(LZ77Pointer left, LZ77Pointer right)
            => left._length < right._length;
        public static bool operator <(LZ77Pointer left, int length)
            => left._length < length;

        public static bool operator <=(LZ77Pointer left, LZ77Pointer right)
            => left._length <= right._length;
        public static bool operator <=(LZ77Pointer left, int length)
            => left._length <= length;
    }
}
