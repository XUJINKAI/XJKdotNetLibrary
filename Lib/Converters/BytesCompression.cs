using System.IO;
using System.IO.Compression;

namespace XJK.Converters
{
    public class BytesCompression : IConverter<byte[], byte[]>
    {
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

        public byte[] Convert(byte[] input)
        {
            using var inputStream = new MemoryStream(input);
            using var targetStream = new MemoryStream();

            // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
            // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
            // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, this approach avoids relying on that very odd behavior should it ever change
            using (var compressorStream = new DeflateStream(targetStream, CompressionLevel, true))
            {
                inputStream.CopyTo(compressorStream);
            }

            // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
            return targetStream.ToArray();
        }

        public byte[] ConvertBack(byte[] output)
        {
            var compressedStream = new MemoryStream(output);

            using var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            decompressorStream.CopyTo(decompressedStream);

            return decompressedStream.ToArray();
        }
    }
}
