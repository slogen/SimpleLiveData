using System.IO;

namespace Scm.Sys.StreamShaping
{
    public class ShapeStreamSums : IStreamShaper
    {
        public ShapeStreamSums(IStreamCost costs, IStreamShedulers schedulers)
        {
            Costs = costs;
            Schedulers = schedulers;
        }
        public IStreamCost Costs { get; }
        public IStreamShedulers Schedulers { get; }

        public Stream ApplyShaping(Stream stream)
            => new ShapedStream<Stream>(stream, Schedulers, Costs);
    }
}