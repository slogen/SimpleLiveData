using System.IO;
using Scm.Sys;

namespace Scm.Rx.Trace
{
    public class TextWriterHereTracer : AbstractHereTracer
    {
        public TextWriterHereTracer(TextWriter writer, ICallerInfo callerInfo, bool? enabled = null) : base(callerInfo,
            enabled)
        {
            Writer = writer;
        }

        public TextWriter Writer { get; }

        protected override void WriteLine(string format, params object[] args)
            => Writer.WriteLine(format, args);
    }
}