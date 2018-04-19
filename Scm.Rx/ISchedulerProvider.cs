using System.Reactive.Concurrency;

namespace Scm.Rx
{
    public interface ISchedulerProvider
    {
        IScheduler Default { get; }

        IScheduler CurrentThread { get; }

        //IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
    }
}