using System.IO;

namespace Scm.Sys.StreamShaping
{
    public static class StreamShaperExtensions {
        public static Stream Shape(this Stream stream, IStreamShaper shaper) => shaper.ApplyShaping(stream);
    }
}