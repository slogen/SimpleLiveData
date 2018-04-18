using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;

namespace Scm.Rx
{
    public class SchedulerProvider: ISchedulerProvider
    {
        private class DefaultSchedulerProvider : ISchedulerProvider
        {
            public static DefaultSchedulerProvider Instance { get; }= new DefaultSchedulerProvider();
            private  DefaultSchedulerProvider() { }
            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Outer not used")]
            public IScheduler Default => ThreadPool;
            public IScheduler CurrentThread => CurrentThreadScheduler.Instance;
            //public IScheduler Dispatcher => Dispatcher
            public IScheduler Immediate => ImmediateScheduler.Instance;
            public IScheduler NewThread => NewThreadScheduler.Default;
            public IScheduler ThreadPool => ThreadPoolScheduler.Instance;
        }
        public static ISchedulerProvider DefaultInstance { get; set; } = DefaultSchedulerProvider.Instance;

        private class SingularSchedulerProvider : ISchedulerProvider
        {
            private readonly IScheduler _scheduler;

            public SingularSchedulerProvider(IScheduler scheduler)
            {
                _scheduler = scheduler;
            }

            public IScheduler Default => _scheduler;

            public IScheduler CurrentThread => _scheduler;

            public IScheduler Immediate => _scheduler;

            public IScheduler NewThread => _scheduler;

            public IScheduler ThreadPool => _scheduler;
        }
        public static ISchedulerProvider Singular(IScheduler scheduler) => new SingularSchedulerProvider(scheduler);
        public IScheduler CurrentThread { get; set; }
        public IScheduler Immediate { get; set; }
        public IScheduler NewThread { get; set; }
        public IScheduler ThreadPool { get; set; }
        public IScheduler Default { get; set; }
    }
}