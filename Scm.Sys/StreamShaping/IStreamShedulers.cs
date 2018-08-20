using Scm.Concurrency;

namespace Scm.Sys.StreamShaping
{
    public interface IStreamShedulers
    {
        IActionScheduler SeekScheduler { get; }
        IActionScheduler ReadScheduler { get; }
        IActionScheduler WriteScheduler { get; }
    }
}