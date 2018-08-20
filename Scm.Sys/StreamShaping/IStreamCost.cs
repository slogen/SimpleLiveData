using System;
using System.IO;

namespace Scm.Sys.StreamShaping
{
    public interface IStreamCost
    {
        int FlushCost { get; }

        int EstimateSeekCost(long offset, SeekOrigin origin);
        int ActualSeekCost(long position, long offset, SeekOrigin origin);
        int ExceptionSeekCost(Exception ex, long offset, SeekOrigin origin);

        int EstimateReadCost(byte[] buffer, int offset, int count);
        int ActualReadCost(int read, byte[] buffer, int offset, int count);
        int ExceptionReadCost(Exception exception, byte[] buffer, int offset, int count);
        int EstimateReadByteCost();
        int ActualReadByteCost(int r);
        int ExceptionReadByteCost(Exception exception);

        int EstimateWriteCost(byte[] buffer, int offset, int count);
        int ActualWriteCost(byte[] buffer, int offset, int count);
        int ExceptionWriteCost(Exception exception, byte[] buffer, int offset, int count);
        int EstimateWriteByteCost();
        int ActualWriteByteCost();
        int ExceptionWriteByteCost(Exception exception);
    }
}