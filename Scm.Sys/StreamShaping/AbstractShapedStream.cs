using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Scm.Concurrency;

namespace Scm.Sys.StreamShaping
{
    public abstract class AbstractShapedStream<TStream> : ProxyStream<TStream>, IStreamShaping
        where TStream: Stream
    {
        public abstract IStreamShedulers Schedulers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public abstract IStreamCost Costs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush()
        {
            Schedulers.SeekScheduler.Schedule(Costs.FlushCost, base.Flush);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
            =>
                Schedulers.ReadScheduler.Schedule(
                    Costs.EstimateReadCost(buffer, offset, count),
                    () => base.Read(buffer, offset, count),
                    r => Costs.ActualReadCost(r, buffer, offset, count),
                    ex => Costs.ExceptionReadCost(ex, buffer, offset, count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin)
            => Schedulers.SeekScheduler.Schedule(
                Costs.EstimateSeekCost(offset, origin), 
                () => base.Seek(offset, origin), 
                p => Costs.ActualSeekCost(p, offset, origin), 
                ex => Costs.ExceptionSeekCost(ex, offset, origin));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count) =>
            Schedulers.WriteScheduler.Schedule(
                Costs.EstimateWriteCost(buffer, offset, count), 
                () => base.Write(buffer, offset, count), 
                () => Costs.ActualWriteCost(buffer, offset, count), 
                ex => Costs.ExceptionWriteCost(ex, buffer, offset, count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            => ReadAsync(buffer, offset, count).ToApm(callback, state);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            => WriteAsync(buffer, offset, count).ToApm(callback, state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract Stream DestinationControl(Stream destination);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyTo(Stream destination, int bufferSize) => base.CopyTo(DestinationControl(destination), bufferSize);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => base.CopyToAsync(DestinationControl(destination), bufferSize, cancellationToken);
        [Obsolete]
        protected override WaitHandle CreateWaitHandle() => throw new NotImplementedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int EndRead(IAsyncResult asyncResult) => ((Task<int>)asyncResult).Result;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndWrite(IAsyncResult asyncResult) => ((Task)asyncResult).Wait();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task FlushAsync(CancellationToken cancellationToken)
            => Schedulers.SeekScheduler.ScheduleAsync(Costs.FlushCost, () => base.FlushAsync(cancellationToken), cancellationToken);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Schedulers.ReadScheduler.ScheduleAsync(
                Costs.EstimateReadCost(buffer, offset, count),
                () => base.ReadAsync(buffer, offset, count, cancellationToken),
                cancellationToken,
                r => Costs.ActualReadCost(r, buffer, offset, count),
                ex => Costs.ExceptionReadCost(ex, buffer, offset, count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int ReadByte() => Schedulers.ReadScheduler.Schedule(Costs.EstimateReadByteCost(), base.ReadByte, Costs.ActualReadByteCost, Costs.ExceptionReadByteCost);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Schedulers.WriteScheduler.ScheduleAsync(
                Costs.EstimateWriteCost(buffer, offset, count), 
                () => base.WriteAsync(buffer, offset, count, cancellationToken), 
                cancellationToken,
                () => Costs.ActualWriteCost(buffer, offset, count), 
                ex => Costs.ExceptionWriteCost(ex, buffer, offset, count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value)
            => Schedulers.WriteScheduler.Schedule(
                Costs.EstimateWriteByteCost(), 
                () => base.WriteByte(value),
                Costs.ActualWriteByteCost,
                Costs.ExceptionWriteByteCost);
    }
}