using System;
using System.IO;

namespace Scm.Sys.StreamShaping
{
    public abstract class AbstractStreamCost : IStreamCost
    {
        public abstract int FlushCost { get; }

        public virtual int ActualReadByteCost(int r) => EstimateReadByteCost();

        public virtual int ActualReadCost(int read, byte[] buffer, int offset, int count) => EstimateReadCost(buffer, offset, count);
        public virtual int ActualSeekCost(long position, long offset, SeekOrigin origin) => EstimateSeekCost(offset, origin);

        public virtual int ActualWriteByteCost() => EstimateWriteByteCost();

        public virtual int ActualWriteCost(byte[] buffer, int offset, int count) => EstimateWriteCost(buffer, offset, count);
        public abstract int EstimateReadByteCost();
        public abstract int EstimateReadCost(byte[] buffer, int offset, int count);
        public abstract int EstimateSeekCost(long offset, SeekOrigin origin);
        public abstract int EstimateWriteByteCost();
        public abstract int EstimateWriteCost(byte[] buffer, int offset, int count);

        public virtual int ExceptionReadByteCost(Exception exception) => EstimateReadByteCost();
        public virtual int ExceptionReadCost(Exception exception, byte[] buffer, int offset, int count) => EstimateReadCost(buffer, offset, count);
        public virtual int ExceptionSeekCost(Exception ex, long offset, SeekOrigin origin) => EstimateSeekCost(offset, origin);
        public virtual int ExceptionWriteByteCost(Exception exception) => EstimateWriteByteCost();

        public virtual int ExceptionWriteCost(Exception exception, byte[] buffer, int offset, int count)
            => EstimateWriteCost(buffer, offset, count);
    }
}