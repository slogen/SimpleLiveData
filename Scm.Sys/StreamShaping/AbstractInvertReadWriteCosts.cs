using System;
using System.IO;

namespace Scm.Sys.StreamShaping
{
    public abstract class AbstractInvertReadWriteCosts : IStreamCost
    {
        public abstract IStreamCost Parent { get; }
        public int FlushCost => Parent.FlushCost;

        public int ActualReadByteCost(int r) => Parent.ActualWriteByteCost();
        public int ActualReadCost(int read, byte[] buffer, int offset, int count)
            => Parent.ActualWriteCost(buffer, offset, count);
        public int ActualSeekCost(long position, long offset, SeekOrigin origin)
            => Parent.ActualSeekCost(position, offset, origin);
        public int ActualWriteByteCost() => Parent.EstimateReadByteCost();
        public int ActualWriteCost(byte[] buffer, int offset, int count)
            => Parent.EstimateReadCost(buffer, offset, count);
        public int EstimateReadByteCost() => Parent.EstimateWriteByteCost();
        public int EstimateReadCost(byte[] buffer, int offset, int count) => Parent.EstimateWriteCost(buffer, offset, count);
        public int EstimateSeekCost(long offset, SeekOrigin origin) => Parent.EstimateSeekCost(offset, origin);
        public int EstimateWriteByteCost() => Parent.EstimateReadByteCost();
        public int EstimateWriteCost(byte[] buffer, int offset, int count) => EstimateReadCost(buffer, offset, count);
        public int ExceptionReadByteCost(Exception exception) => ExceptionWriteByteCost(exception);
        public int ExceptionReadCost(Exception exception, byte[] buffer, int offset, int count)
            => Parent.ExceptionWriteCost(exception, buffer, offset, count);
        public int ExceptionSeekCost(Exception ex, long offset, SeekOrigin origin)
            => Parent.ExceptionSeekCost(ex, offset, origin);
        public int ExceptionWriteByteCost(Exception exception)
            => Parent.ExceptionReadByteCost(exception);
        public int ExceptionWriteCost(Exception exception, byte[] buffer, int offset, int count)
            => Parent.ExceptionReadCost(exception, buffer, offset, count);
    }
}