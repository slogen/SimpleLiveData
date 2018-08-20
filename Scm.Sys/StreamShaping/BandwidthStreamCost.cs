using System;
using System.IO;

namespace Scm.Sys.StreamShaping
{
    public class BandwidthStreamCost: AbstractStreamCost
    {
        public static BandwidthStreamCost Default { get; set; } = new BandwidthStreamCost();
        public override int EstimateReadByteCost() => 1;
        public override int EstimateReadCost(byte[] buffer, int offset, int count) => count;
        public override int EstimateSeekCost(long offset, SeekOrigin origin) => 1;
        public override int EstimateWriteByteCost() => 1;
        public override int EstimateWriteCost(byte[] buffer, int offset, int count) => count;
        public override int FlushCost => 1;
        public override int ActualReadCost(int read, byte[] buffer, int offset, int count) => Math.Max(read, 1);
    }
}