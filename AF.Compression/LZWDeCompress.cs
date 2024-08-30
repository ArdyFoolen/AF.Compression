using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    internal class LZWDeCompress
    {
        private Dictionary<byte[], UInt16> table = new Dictionary<byte[], UInt16>(new ByteArrayComparer());
        private TwelveBitIterator twelveBitIterator;

        private UInt16 nextCode = 256;

        //private UInt16 code = 0;
        //private UInt16 old = 0;
        //private byte K;
        private bool first;

        private ILZWDiagnoser? diagnoser;

        public LZWDeCompress(ILZWDiagnoser? diagnoser = null)
        {
            this.diagnoser = diagnoser;
        }

        private UInt16 _old;
        private UInt16 old
        {
            get => _old;
            set
            {
                _old = value;
                if (diagnoser != null)
                    diagnoser.Write($"Old: {_old.ToString("x3")}");
            }
        }
        private UInt16 _code;
        private UInt16 code
        {
            get => _code;
            set
            {
                _code = value;
                if (diagnoser != null)
                    diagnoser.Write($"Code: {_code.ToString("x3")}");
            }
        }
        private byte _k;
        private byte K
        {
            get => _k;
            set
            {
                _k = value;
                if (diagnoser != null)
                    diagnoser.Write($"K: {_k.ToString("x2")}");
            }
        }
        private byte[] _prefix;
        private byte[] Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                if (diagnoser != null)
                {
                    diagnoser.Write("Prefix: [");
                    foreach (byte b in _prefix) { diagnoser.Write(b.ToString("x2")); }
                    diagnoser.Write("]");
                }
            }
        }

        public void InitializeTable(TwelveBitIterator twelveBitIterator)
        {
            this.twelveBitIterator = twelveBitIterator;

            nextCode = 256;
            code = 0;
            first = true;

            table.Clear();
            for (UInt16 i = 0; i < 256; i++)
                table.Add(new byte[] { (byte)i }, i);
        }

        public IEnumerable<char> Write()
        {
            foreach (UInt16 code in twelveBitIterator)
            {
                this.code = code;

                if (ContainsValue(code))
                    foreach (char c in WriteInternal())
                        yield return c;
                else
                    foreach (char c in WriteOld())
                        yield return c;

                AddNextCode();
                old = code;

                diagnoser?.NextLoop();
            }
        }

        private IEnumerable<byte> WriteInternal()
        {
            byte[] value;
            value = GetKey(code);
            K = value[0];
            foreach (byte c in value)
                yield return c;
        }

        private IEnumerable<byte> WriteOld()
        {
            byte[] value;
            value = GetKey(old);
            K = value[0];
            foreach (byte c in value)
                yield return c;
            yield return K;
        }

        private bool ContainsValue(UInt16 code)
            => table.FirstOrDefault(kv => kv.Value == code).Key != null;
        private byte[] GetKey(UInt16 code)
            => table.First(kv => kv.Value == code).Key;

        private void AddNextCode()
        {
            Prefix = GetKey(old);
            if (!first && !ContainsKey(Prefix, K))
                Add(Prefix, K);
            first = false;
        }

        private bool ContainsKey(byte[] prefix, byte nextChar)
            => table.ContainsKey(prefix.Concat(nextChar));

        private void Add(byte[] prefix, byte nextChar)
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
    }
}
