using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Sys
{
    public static class MemoryStreamExtensions
    {
        public static int DefaultBufferSize = 81920;
        public static MemoryStream AsStream(this byte[] bytes, int? index = null, int? count = null,
            bool? writeable = null)
            => new MemoryStream(bytes, index ?? 0, count ?? bytes.Length - (index ?? 0), writeable ?? true);
        public static async Task<MemoryStream> AsMemoryStreamAsync(
            this Task<byte[]> bytesTask,
            Func<byte[], int> index = null,
            Func<byte[], int> count = null,
            Func<byte[], bool> writeable = null)
        {
            var bytes = await bytesTask.ConfigureAwait(false);
            return bytes.AsStream(index?.Invoke(bytes), count?.Invoke(bytes), writeable?.Invoke(bytes));
        }

        public static Stream AsSeekable(this Stream stream, int? bufferSize = null)
        {
            if (stream.CanSeek)
                return stream;
            var m = new MemoryStream();
            stream.CopyTo(m, bufferSize ?? DefaultBufferSize);
            m.Seek(0, SeekOrigin.Begin);
            return m;
        }
        public static async Task<Stream> AsSeekableAsync(this Stream stream, CancellationToken cancellationToken, int? bufferSize = null)
        {
            if (stream.CanSeek)
                return stream;
            var m = new MemoryStream();
            await stream.CopyToAsync(m, bufferSize ?? DefaultBufferSize, cancellationToken).ConfigureAwait(false);
            m.Seek(0, SeekOrigin.Begin);
            return m;
        }

        public static async Task<Stream> AsSeekableAsync(this Task<Stream> stream, CancellationToken cancellationToken,
            int? bufferSize = null)
         => await AsSeekableAsync(await stream.ConfigureAwait(false), cancellationToken, bufferSize).ConfigureAwait(false);
    }
}
