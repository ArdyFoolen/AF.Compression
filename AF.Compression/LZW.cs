using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;
using System.Transactions;
using System.Xml;
using System.IO;

namespace AF.Compression
{
    public class LZW
    {
        private LZWCompress lZWCompress;
        private LZWDeCompress lZWDeCompress;

        private ILZWDiagnoser? diagnoser;

        public LZW(ILZWDiagnoser? diagnoser = null)
        {
            this.diagnoser = diagnoser;
            lZWCompress = new LZWCompress(diagnoser);
            lZWDeCompress = new LZWDeCompress(diagnoser);
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

        // https://www.eecis.udel.edu/~amer/CISC651/lzw.and.gif.explained.html
        // [1] Initialize string table;
        // [2][.c.] <- empty;
        // [3] K<- next character in charstream;
        // [4] Is[.c.] K in string table?
        //     (yes: [.c.] <- [.c.] K;
        //           go to[3];
        //     )
        //     (no: add[.c.] K to the string table;
        //          output the code for [.c.] to the codestream;
        //          [.c.] <- K;
        //          go to[3];
        //     )
        // [5] output the code for [.c.] to the codestream;

        //PSEUDOCODE
        //  1     Initialize table with single character strings
        //  2     P = first input character
        //  3     WHILE not end of input stream
        //  4          C = next input character
        //  5          IF P + C is in the string table
        //  6            P = P + C
        //  7          ELSE
        //  8            output the code for P
        //  9          add P + C to the string table
        //  10           P = C
        //  11         END WHILE
        //  12    output code for P
        public IEnumerable<byte> Compress(IEnumerable<byte> text)
        {
            lZWCompress.InitializeTable();
            Prefix = new byte[0];

            foreach (byte c in text)
            {
                if (lZWCompress.ContainsKey(Prefix, c))
                    Prefix = Prefix.Concat(c);
                else
                {
                    lZWCompress.Add(Prefix, c);
                    foreach (var b in lZWCompress.Write(Prefix))
                        yield return b;
                    Prefix = new byte[] { c };
                }
                diagnoser?.NextLoop();
            }
            foreach (var b in lZWCompress.Write(Prefix, true))
                yield return b;
        }

        // [1] Initialize string table;
        // [2] get first code: <old>
        // [3] output the string for <old> to the charstream;
        // [4] <code> <- next code in codestream;
        // [5] does<code> exist in the string table?
        //    (yes: output the string for <code> to the charstream;
        //       [...] <- translation for <old>
        //       K<- first character of translation for <code>
        //       add[...]K to the string table;        
        //    )
        //    (no: [...] <- translation for <old>
        //       K<- first character of[...];
        //       output[...] K to charstream and add it to string table;
        //    )
        // [6] <old> <- <code>
        // [7] go to[4];

        //PSEUDOCODE
        //   1    Initialize table with single character strings
        //   2    OLD = first input code
        //   3    output translation of OLD
        //   4    WHILE not end of input stream
        //   5        NEW = next input code
        //   6        IF NEW is not in the string table
        //   7               S = translation of OLD
        //   8               S = S + C
        //   9       ELSE
        //   10              S = translation of NEW
        //   11       output S
        //   12       C = first character of S
        //   13       OLD + C to the string table
        //   14       OLD = NEW
        //   15   END WHILE
        public IEnumerable<byte> DeCompress(IEnumerable<byte> data)
        
        {
            lZWDeCompress.InitializeTable(new TwelveBitIterator(data));

            foreach (byte c in lZWDeCompress.Write())
                yield return c;
        }
    }
}