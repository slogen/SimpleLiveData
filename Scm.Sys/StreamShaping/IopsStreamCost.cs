using System.IO;

namespace Scm.Sys.StreamShaping
{
    public class IopsStreamCost: AbstractStreamCost
    {
        public static IopsStreamCost Default { get; set; } = new IopsStreamCost();
        public override int EstimateReadByteCost() => 1;
        public override int EstimateReadCost(byte[] buffer, int offset, int count) => 1;
        public override int EstimateSeekCost(long offset, SeekOrigin origin) => 1;
        public override int EstimateWriteByteCost() => 1;
        public override int EstimateWriteCost(byte[] buffer, int offset, int count) => 1;
        public override int FlushCost => 1;
    }
}