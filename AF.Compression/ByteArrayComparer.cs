﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    internal class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            return left.SequenceEqual(right);
        }
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return key.Length;
        }
    }
}
