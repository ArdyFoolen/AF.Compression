LZW compression works by reading a sequence of symbols, grouping the symbols into strings, and converting the strings into codes. Because the codes take up less space than the strings they replace, we get compression. Characteristic features of LZW includes, 

LZW compression uses a code table, with 4096 as a common choice for the number of table entries. Codes 0-255 in the code table are always assigned to represent single bytes from the input file.
When encoding begins the code table contains only the first 256 entries, with the remainder of the table being blanks. Compression is achieved by using codes 256 through 4095 to represent sequences of bytes.
As the encoding continues, LZW identifies repeated sequences in the data and adds them to the code table.
Decoding is achieved by taking each code from the compressed file and translating it through the code table to find what character or characters it represents.
Example: ASCII code. Typically, every character is stored with 8 binary bits, allowing up to 256 unique symbols for the data. This algorithm tries to extend the library to 9 to 12 bits per character. The new unique symbols are made up of combinations of symbols that occurred previously in the string. It does not always compress well, especially with short, diverse strings. But is good for compressing redundant data, and does not have to save the new dictionary with the data: this method can both compress and uncompress data. 
There are excellent article’s written up already, you can look more in-depth here, and also Mark Nelson’s article is commendable. 

Implementation

The idea of the compression algorithm is the following: as the input data is being processed, a dictionary keeps a correspondence between the longest encountered words and a list of code values. The words are replaced by their corresponding codes and so the input file is compressed. Therefore, the efficiency of the algorithm increases as the number of long, repetitive words in the input data increases.

LZW ENCODING

  *     PSEUDOCODE
  1     Initialize table with single character strings
  2     P = first input character
  3     WHILE not end of input stream
  4          C = next input character
  5          IF P + C is in the string table
  6            P = P + C
  7          ELSE
  8            output the code for P
  9          add P + C to the string table
  10           P = C
  11         END WHILE
  12    output code for P 

Compression using LZW

Example 1: Use the LZW algorithm to compress the string: BABAABAAA 
The steps involved are systematically shown in the diagram below. 

P	C (c)	Output	New entry
B	A		<66>	<256> BA
A	B		<65>	<257> AB
B	A
BA	A		<256>	<258> BAA
A	B
AB	A		<257>	<259> ABA
A	A		<65>	<260> AA
A	A
AA			<260>

LZW Decompression

The LZW decompressor creates the same string table during decompression. It starts with the first 256 table entries initialized to single characters. The string table is updated for each character in the input stream, except the first one. Decoding is achieved by reading codes and translating them through the code table being built.

LZW Decompression Algorithm

*    PSEUDOCODE
1    Initialize table with single character strings
2    OLD = first input code
3    output translation of OLD
4    WHILE not end of input stream
5        NEW = next input code
6        IF NEW is not in the string table
7               S = translation of OLD
8               S = S + C
9       ELSE
10              S = translation of NEW
11       output S
12       C = first character of S
13       OLD + C to the string table
14       OLD = NEW
15   END WHILE

Example 2: LZW Decompression: Use LZW to decompress the output sequence of : <66><65><256><257><65><260> 
The steps involved are systematically shown in the diagram below.

OLD		OUTPUT	NEW		S	C	New entry
<66>	BA		<65>	A	A	<256> BA
<65>	BA		<256>	BA	B	<257> AB
<256>	AB		<257>	AB	A	<258> BAA
<257>	A		<65>	A	A	<259> ABA
<65>	AA		<260>	AA	A	<260> AA
<260>

In this example, 72 bits are represented with 72 bits of data. After a reasonable string table is built, compression improves dramatically. 
LZW Summary: This algorithm compresses repetitive sequences of data very well. Since the codewords are 12 bits, any single encoded character will expand the data size rather than reduce it.

Advantages of LZW over Huffman: 

LZW requires no prior information about the input data stream.
LZW can compress the input stream in one single pass.
Another advantage of LZW is its simplicity, allowing fast execution.
High Compression Ratio: LZW can achieve high compression ratios, particularly for text-based data, which can significantly reduce file sizes and storage requirements.

Fast Decompression: LZW decompression is typically faster than other compression algorithms, making it a good choice for applications where decompression speed is critical.

Universal Adoption: LZW is widely used and supported across a variety of software applications and operating systems, making it a popular choice for compression and decompression.

Dynamic Compression: LZW uses a dynamic compression algorithm, meaning it adapts to the data being compressed, which allows it to achieve high compression ratios even for data with repetitive patterns.

Disadvantages:

Patent Issues: LZW compression was patented in the 1980s, and for many years its use was subject to licensing fees, which limited its adoption in some applications.

Memory Requirements: LZW compression requires significant memory to maintain the compression dictionary, which can be a problem for applications with limited memory resources.

Compression Speed: LZW compression can be slower than some other compression algorithms, particularly for large files, due to the need to constantly update the dictionary.

Limited Applicability: LZW compression is particularly effective for text-based data, but may not be as effective for other types of data, such as images or video, which have different compression requirements.
