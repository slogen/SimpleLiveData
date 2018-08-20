using Scm.Concurrency;

namespace Scm.Sys.StreamShaping
{
    public class InvertReadWriteSchedulers : IStreamShedulers
    {
        public IStreamShedulers Parent { get; }
        public InvertReadWriteSchedulers(IStreamShedulers parent) { Parent = parent; }
        public IActionScheduler SeekScheduler => Parent.SeekScheduler;

        public IActionScheduler ReadScheduler => Parent.WriteScheduler;

        public IActionScheduler WriteScheduler => Parent.WriteScheduler;
    }
}