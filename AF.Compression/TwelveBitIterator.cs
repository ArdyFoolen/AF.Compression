using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    internal class TwelveBitIterator : IEnumerable<UInt16>
    {
        private IEnumerable<byte> bytes;
        private bool saveHalf;
        private UInt16 halfSaved;

        public TwelveBitIterator(IEnumerable<byte> bytes)
        {
            this.bytes = bytes;
        }

        public IEnumerator<UInt16> GetEnumerator()
        {
            saveHalf = true;
            halfSaved = 0;
            UInt16 code;

            var enumerator = bytes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (saveHalf)
                {
                    byte[] codeBuffer = new byte[2];

                    codeBuffer[0] = enumerator.Current;
                    enumerator.MoveNext();
                    codeBuffer[1] = enumerator.Current;

                    code = BitConverter.ToUInt16(codeBuffer.Reverse().ToArray(), 0);
                    halfSaved = (UInt16)((code & 0x000F) << 8);
                    code = (UInt16)(code >> 4);
                }
                else
                {
                    code = (UInt16)(halfSaved | enumerator.Current);
                    halfSaved = 0x0000;
                }
                saveHalf = !saveHalf;

                yield return code;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
