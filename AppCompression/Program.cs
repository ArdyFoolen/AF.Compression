using AF.Compression;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

CompressDecompressLZ77_3("TextInput01.txt", "TextCompressLZ77-01.zip", "TextDeCompressLZ77-01.txt");

CompressDecompressLZ77_2("aaaxbbbycccz");
CompressDecompressLZ77_2("abababxcdcdcdyefefefz");
CompressDecompressLZ77_2("xyxyzxyzkxyzklxyzklmxyzklmnxyzklm");
CompressDecompressLZ77_2("This is ");
CompressDecompressLZ77_2("This is my string to compress and see its length.");
CompressDecompressLZ77_2("ababbacdaba");
CompressDecompressLZ77_2("daar daar daar daar daar");
CompressDecompressLZ77_2("ababaabaaba");

CompressDecompressLZ77(new byte[] { 0x41, 0x42, 0x41, 0x42, 0x41, 0x41, 0x42, 0x41, 0x41, 0x42, 0x42 });
CompressDecompressLZ77(new byte[] { 0x00, 0x00, 0x00, 0x41, 0x41, 0x41, 0x00, 0x00, 0x00, 0x42, 0x42 });

CompressDecompress4("TextInput01.txt", "TextCompressDiag01.zip", "TextDeCompressDiag01.txt");
CompressDecompress3("TextInput01.txt", "TextCompress01.zip", "TextDeCompress01.txt");

CompressDecompress2(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
CompressDecompress2(new byte[] { 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01 });
CompressDecompress2(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01 });

CompressDecompress("aaaxbbbycccz");
CompressDecompress("abababxcdcdcdyefefefz");
CompressDecompress("xyxyzxyzkxyzklxyzklmxyzklmnxyzklm");
CompressDecompress("This is ");
CompressDecompress("This is my string to compress and see its length.");
CompressDecompress("ababbacdaba");
CompressDecompress("daar daar daar daar daar");
CompressDecompress("ababaabaaba");

static void CompressDecompressLZ77(byte[] text)
{
    LZ77 lZ77 = new LZ77();
    byte[] result = lZ77.Compress(text).ToArray();

    string hex = BitConverter.ToString(text);
    Console.WriteLine(hex);
    hex = BitConverter.ToString(result);
    Console.WriteLine(hex);

    byte[] data = lZ77.DeCompress(result).ToArray();
    hex = BitConverter.ToString(data);
    Console.WriteLine(hex);
}
static void CompressDecompressLZ77_2(string text)
{
    LZ77 lZ77 = new LZ77();
    byte[] bytes = Encoding.ASCII.GetBytes(text);

    byte[] result = lZ77.Compress(bytes).ToArray();
    string hex = BitConverter.ToString(result);
    Console.WriteLine(text);
    Console.WriteLine(hex);
    Console.WriteLine($"String length: {text.Length}, compress length: {result.Length}");

    string resultText = Encoding.ASCII.GetString(lZ77.DeCompress(result).ToArray());

    Console.WriteLine(resultText);
}

static void CompressDecompressLZ77_3(string inputPath, string compressedPath, string decompressedPath)
{
    LZ77 lZ77 = new LZ77();
    IEnumerable<byte> compressed = lZ77.Compress(InputStream(inputPath));
    OutputStream(compressedPath, compressed);
    IEnumerable<byte> decompressed = lZ77.DeCompress(InputStream(compressedPath));
    OutputStream(decompressedPath, decompressed);
}

static void CompressDecompress(string text)
{
    LZW lZW = new LZW();
    byte[] bytes = Encoding.ASCII.GetBytes(text);

    byte[] result = lZW.Compress(bytes).ToArray();
    string hex = BitConverter.ToString(result);
    Console.WriteLine(text);
    Console.WriteLine(hex);
    Console.WriteLine($"String length: {text.Length}, compress length: {result.Length}");

    string resultText = Encoding.ASCII.GetString(lZW.DeCompress(result).ToArray());

    Console.WriteLine(resultText);
}

static void CompressDecompress2(byte[] text)
{
    LZW lZW = new LZW();

    Console.WriteLine("---------------------------------------------------------------");
    string hex = BitConverter.ToString(text);
    Console.WriteLine($"Before: {hex}");

    byte[] result = lZW.Compress(text).ToArray();
    hex = BitConverter.ToString(result);
    Console.WriteLine(hex);
    Console.WriteLine($"Input length: {text.Length}, compress length: {result.Length}");
    byte[] resultText = lZW.DeCompress(result).ToArray();

    hex = BitConverter.ToString(resultText);
    Console.WriteLine($"After : {hex}");
    Console.WriteLine("---------------------------------------------------------------");
}

static void CompressDecompress3(string inputPath, string compressedPath, string decompressedPath)
{
    LZW lZW = new LZW();
    IEnumerable<byte> compressed = lZW.Compress(InputStream(inputPath));
    OutputStream(compressedPath, compressed);
    IEnumerable<byte> decompressed = lZW.DeCompress(InputStream(compressedPath));
    OutputStream(decompressedPath, decompressed);
}

static void CompressDecompress4(string inputPath, string compressedPath, string decompressedPath)
{
    LZWDiagnoser lZW = new LZWDiagnoser();
    IEnumerable<byte> compressed = lZW.Compress(InputStream(inputPath));
    OutputStream(compressedPath, compressed);
    IEnumerable<byte> decompressed = lZW.DeCompress(InputStream(compressedPath));
    OutputStream(decompressedPath, decompressed);
}

static IEnumerable<byte> InputStream(string inputPath)
{
    int bufferSize = 10;
    byte[] buffer = new byte[bufferSize];
    using (var stream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
    {
        int read = stream.Read(buffer, 0, bufferSize);
        while (read != 0)
        {
            for (int i = 0; i < read; i++)
                yield return buffer[i];

            read = stream.Read(buffer, 0, 10);
        }
    }
}

static void OutputStream(string outputPath, IEnumerable<byte> inputStream)
{
    int index = 0;
    int bufferSize = 10;
    byte[] buffer = new byte[bufferSize];
    using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    {
        foreach (byte b in inputStream)
        {
            buffer[index++] = b;
            if (index == bufferSize)
            {
                stream.Write(buffer, 0, bufferSize);
                index = 0;
            }
        }
        stream.Write(buffer, 0, index);
    }
}
