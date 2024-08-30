using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AF.Compression
{
    public interface ILZWDiagnoser
    {
        void Write(string text);
        void NextLoop();
    }

    public class LZWDiagnoser : ILZWDiagnoser
    {
        LZW lZW;

        public LZWDiagnoser()
        {
            lZW = new LZW(this);
        }

        public IEnumerable<byte> Compress(IEnumerable<byte> text)
        {
            Console.WriteLine("----- Compress -----");
            foreach (byte b in lZW.Compress(Input(text)))
                yield return b;
        }

        public IEnumerable<byte> DeCompress(IEnumerable<byte> data)
        {
            Console.WriteLine("---- Decompress ----");
            foreach (byte c in lZW.DeCompress(data))
            {
                Write($"Output: {c.ToString("x2")}");
                yield return c;
            }
        }

        private IEnumerable<byte> Input(IEnumerable<byte> text)
        {
            foreach (byte b in text)
            {
                Write($"Input: {b.ToString("x2")}");
                yield return b;
            }
        }

        public void Write(string text)
        {
            Console.Write($"{text} ");
        }

        public void NextLoop()
        {
            Console.WriteLine("");
        }
    }
}
