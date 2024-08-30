using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    public static class Extensions
    {
        public static byte[] GetBytes(this uint value)
        {
            uint bigEndian = (uint)IPAddress.HostToNetworkOrder((int)value);
            return BitConverter.GetBytes(bigEndian);
        }

        public static string GetString(this UInt16 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byte[] bytes2 = bytes.Reverse().ToArray();
            char[] chars = bytes2.Select(s => (char)s).ToArray();
            return new string(BitConverter.GetBytes(value).Reverse().Select(s => (char)s).ToArray());
        }

        public static byte[] Compress16To12Bits(this byte[] data)
        {
            decimal length = (decimal)data.Length * 3 / 4;
            byte[] newBuffer = new byte[length.Ceiling()];
            int outIndex = 0;
            for (int inIndex = 0; inIndex < data.Length; inIndex += 4)
            {
                newBuffer[outIndex] = (byte)(data[inIndex] << 4);
                if (inIndex + 1 < data.Length)
                {
                    newBuffer[outIndex] |= (byte)(data[inIndex + 1] >> 4);
                    newBuffer[outIndex + 1] = (byte)(data[inIndex + 1] << 4);
                    if ((inIndex + 2) < data.Length)
                    {
                        newBuffer[outIndex + 1] |= data[inIndex + 2];
                        if (inIndex + 3 < data.Length)
                            newBuffer[outIndex + 2] = data[inIndex + 3];
                    }
                }

                outIndex += 3;
            }
            return newBuffer;
        }

        public static byte[] Expand12To16Bits(this byte[] data)
        {
            decimal length = (decimal)data.Length * 4 / 3;
            byte[] newBuffer = new byte[length.Ceiling()];
            int outIndex = 0;
            for (int inIndex = 0; inIndex < data.Length; inIndex += 3)
            {
                newBuffer[outIndex] |= (byte)(data[inIndex] >> 4);
                newBuffer[outIndex + 1] = (byte)(data[inIndex] << 4);
                if (inIndex + 1 < data.Length)
                {
                    newBuffer[outIndex + 1] |= (byte)(data[inIndex + 1] >> 4);
                    newBuffer[outIndex + 2] = (byte)(data[inIndex + 1] & 0x0F);
                    if (inIndex + 2 < data.Length)
                        newBuffer[outIndex + 3] = data[inIndex + 2];
                }

                outIndex += 4;
            }
            return newBuffer;
        }

        public static byte[] Concat(this byte[] prefix, byte value)
            => prefix.Concat(new byte[] { value }).ToArray();
        public static int Ceiling(this decimal value)
            => (int)Math.Ceiling(value);
    }
}
