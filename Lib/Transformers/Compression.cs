using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace XJK
{
    public static class Compression
    {
        public static string CompressAction(Action action)
        {
            return CompressString(BinarySerialization.ToBase64BinaryString(action));
        }

        public static Action DecompressAction(string compressedString)
        {
            return BinarySerialization.FromBase64BinaryString<object>(DecompressString(compressedString)) as Action;
        }

        public static string CompressString(string uncompressedString, CompressionLevel CompressionLevel = CompressionLevel.Optimal)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                var compressedStream = new MemoryStream();

                // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, this approach avoids relying on that very odd behavior should it ever change
                using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel, true))
                {
                    uncompressedStream.CopyTo(compressorStream);
                }

                // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                compressedBytes = compressedStream.ToArray();
            }

            return Convert.ToBase64String(compressedBytes);
        }

        public static string DecompressString(string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }
    }
}
