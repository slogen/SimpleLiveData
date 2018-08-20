using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Sys.StreamShaping
{
    public class ReadMaxStream<TStream> : ProxyStream<TStream>
        where TStream : Stream
    {
        public override TStream Parent { get; }
        public int MaxRead { get; set; }

        public ReadMaxStream(TStream parent, int maxRead)
        {
            Parent = parent;
            MaxRead = maxRead;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
            => base.BeginRead(buffer, offset, Math.Min(count, MaxRead), callback, state);

        public override int Read(byte[] buffer, int offset, int count)
         => base.Read(buffer, offset, Math.Min(count, MaxRead));

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => base.ReadAsync(buffer, offset, Math.Min(count, MaxRead), cancellationToken);

        public override void CopyTo(Stream destination, int bufferSize)
        {
            var buf = new byte[Math.Min(bufferSize, MaxRead)];
            int r;
            while ((r = Read(buf, 0, buf.Length)) > 0)
                destination.Write(buf, 0, r);
        }

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            var buf = new byte[Math.Min(bufferSize, MaxRead)];
            int r;
            while ((r = await ReadAsync(buf, 0, buf.Length, cancellationToken).ConfigureAwait(false)) > 0)
                await destination.WriteAsync(buf, 0, r, cancellationToken).ConfigureAwait(false);
        }
    }
}
