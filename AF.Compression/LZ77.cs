using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AF.Compression
{
    public class LZ77
    {
        public const int WindowSize = 32;
        public const int MaxLength = 255;
        public IEnumerable<byte> Compress(IEnumerable<byte> text)
        {
            int currentPosition = 0;

            IEnumerable<byte> previousWindow = text.Take(currentPosition);
            IEnumerator<byte> currentWindow = text.GetEnumerator();

            while (currentWindow.MoveNext())
            {
                byte b = currentWindow.Current;
                int[] indexes = previousWindow
                    .Select((s, i) => new { s, i })
                    .Where(w => w.s == b)
                    .Select(x => x.i)
                    .ToArray();

                LZ77Pointer pointer = new LZ77Pointer(0, 0);
                foreach (int index in indexes)
                {
                    IEnumerator<byte> nextWindow = text.Skip(currentPosition).GetEnumerator();
                    int c = 0;
                    IEnumerator<byte> iter = previousWindow.Skip(index).GetEnumerator();
                    while (iter.MoveNext() && nextWindow.MoveNext() && iter.Current == nextWindow.Current && c < MaxLength)
                        c += 1;
                    if (pointer <= c)
                    {
                        byte offset = currentPosition < WindowSize ? (byte)(currentPosition - index) : (byte)(WindowSize - index);
                        pointer = new LZ77Pointer(offset, (byte)c);
                    }
                }

                if (pointer > 1)
                {
                    for (int i = 1; i < pointer.Length; i++)
                    {
                        currentWindow.MoveNext();
                        currentPosition += 1;
                    }

                    yield return pointer.Offset;
                    yield return pointer.Length;
                }
                else
                {
                    yield return (byte)0x00;
                    yield return b;
                }

                currentPosition += 1;
                int take = currentPosition < WindowSize ? currentPosition : WindowSize;
                int skip = currentPosition < WindowSize ? 0 : currentPosition - WindowSize;
                previousWindow = text.Skip(skip).Take(take);
            }
        }

        public IEnumerable<byte> DeCompress(IEnumerable<byte> data)
        {
            LZ77SlidingWindow previousWindow = new LZ77SlidingWindow();
            IEnumerator<byte> currentWindow = data.GetEnumerator();

            while (currentWindow.MoveNext())
            {
                byte b = currentWindow.Current;
                if (b == 0x00)
                {
                    currentWindow.MoveNext();
                    b = currentWindow.Current;
                    previousWindow = previousWindow.Push(b);
                    yield return b;
                }
                else
                {
                    currentWindow.MoveNext();
                    LZ77Pointer pointer = new LZ77Pointer(b, currentWindow.Current);
                    int skip = previousWindow.Count() - pointer.Offset;
                    IEnumerable<LZ77SlidingWindow> iter = previousWindow.Skip(skip).Take(pointer.Length);
                    foreach (LZ77SlidingWindow window in iter)
                    {
                        previousWindow = previousWindow.Push(window.Value);
                        yield return window.Value;
                    }
                }
            }
        }
    }
}
