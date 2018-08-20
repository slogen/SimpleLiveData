using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Sys
{

    public abstract class ProxyStream<TStream> : Stream
        where TStream: Stream
    {
        public abstract TStream Parent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public override bool CanRead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.CanRead;
        }

        public override bool CanSeek
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.CanSeek;
        }

        public override bool CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.CanWrite;
        }

        public override long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.Length;
        }

        public override long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.Position;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Parent.Position = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush() => Parent.Flush();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count) => Parent.Read(buffer, offset, count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin) => Parent.Seek(offset, origin);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetLength(long value) => Parent.SetLength(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count) => Parent.Write(buffer, offset, count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            => Parent.BeginRead(buffer, offset, count, callback, state);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            => Parent.BeginWrite(buffer, offset, count, callback, state);

        public override bool CanTimeout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.CanTimeout;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Close() => Parent.Close();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyTo(Stream destination, int bufferSize) 
            => Parent.CopyTo(destination, bufferSize);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) 
            => Parent.CopyToAsync(destination, bufferSize, cancellationToken);
        [Obsolete]
        protected override WaitHandle CreateWaitHandle() => throw new NotImplementedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Dispose(bool disposing) { if (disposing) { Parent.Dispose(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int EndRead(IAsyncResult asyncResult) => Parent.EndRead(asyncResult);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndWrite(IAsyncResult asyncResult) => Parent.EndWrite(asyncResult);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => Parent.Equals(obj);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task FlushAsync(CancellationToken cancellationToken) => Parent.FlushAsync(cancellationToken);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Parent.GetHashCode();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object InitializeLifetimeService() => Parent.InitializeLifetimeService();
        [Obsolete]
        protected override void ObjectInvariant() => throw new NotImplementedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Parent.ReadAsync(buffer, offset, count, cancellationToken);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int ReadByte() => Parent.ReadByte();
        public override int ReadTimeout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.ReadTimeout;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Parent.ReadTimeout = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => Parent.ToString();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) 
            => Parent.WriteAsync(buffer, offset, count, cancellationToken);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value) => Parent.WriteByte(value);

        public override int WriteTimeout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Parent.WriteTimeout;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Parent.WriteTimeout = value;
        }
    }
}
