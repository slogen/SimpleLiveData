using System.IO;

namespace Scm.Sys.StreamShaping
{
    public class ShapedStream<TStream> : AbstractShapedStream<TStream>
        where TStream: Stream
    {
        public ShapedStream(TStream parent, IStreamShedulers schedulers, IStreamCost costs) {
            Parent = parent;
            Schedulers = schedulers;
            Costs = costs;
        }

        public override TStream Parent { get; }
        public override IStreamShedulers Schedulers { get; }
        public override IStreamCost Costs { get; }

        protected override Stream DestinationControl(Stream destination)
        {
            return new ShapedStream<Stream>(destination, new InvertReadWriteSchedulers(Schedulers), new InvertReadWriteCosts(Costs));
        }
    }
}