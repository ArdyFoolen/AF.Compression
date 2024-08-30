using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    internal class LZWCompress
    {
        private Dictionary<byte[], UInt16> table = new Dictionary<byte[], UInt16>(new ByteArrayComparer());
        private UInt16 nextCode = 256;

        private bool saveHalf;
        private UInt16 halfSaved = 0;

        private ILZWDiagnoser? diagnoser;

        public LZWCompress(ILZWDiagnoser? diagnoser = null)
        {
            this.diagnoser = diagnoser;
        }

        public void InitializeTable()
        {
            saveHalf = true;
            nextCode = 256;
            halfSaved = 0;

            table.Clear();
            for (UInt16 i = 0; i < 256; i++)
                table.Add(new byte[] { (byte)i }, i);
        }

        public bool ContainsKey(byte[] prefix, byte nextChar)
            => table.ContainsKey(prefix.Concat(nextChar));

        public UInt16 GetKey(byte[] prefix, byte nextChar)
        {
            if (ContainsKey(prefix, nextChar))
                return table[prefix.Concat(nextChar)];
            throw new ArgumentOutOfRangeException();
        }

        public void Add(byte[] prefix, byte nextChar)
        {
            byte[] concatted = prefix.Concat(nextChar);
            if (diagnoser != null)
            {
                diagnoser.Write("Add: [");
                foreach (byte b in concatted) { diagnoser.Write(b.ToString("x2")); }
                diagnoser.Write($"] {nextCode.ToString("x3")}");
            }
            table.Add(concatted, nextCode++);
        }

        public IEnumerable<byte> Write(byte[] prefix, bool flush = false)
        {
            if (diagnoser != null && !flush)
                diagnoser.Write($"Write: {table[prefix].ToString("x3")}");

            if (saveHalf)
            {
                yield return (byte)(table[prefix] >> 4);
                halfSaved = (UInt16)((table[prefix] & 0x000F) << 4);
                if (flush)
                    yield return (byte)halfSaved;
            }
            else
            {
                yield return (byte)(table[prefix] >> 8 | halfSaved);
                yield return (byte)(table[prefix] & 0x00FF);
                halfSaved = 0;
            }
            saveHalf = !saveHalf;
        }
    }
}
