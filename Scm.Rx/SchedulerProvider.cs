using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;

namespace Scm.Rx
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public static ISchedulerProvider DefaultInstance { get; set; } = DefaultSchedulerProvider.Instance;
        public IScheduler CurrentThread { get; set; }
        public IScheduler Immediate { get; set; }
        public IScheduler NewThread { get; set; }
        public IScheduler ThreadPool { get; set; }
        public IScheduler Default { get; set; }
        public static ISchedulerProvider Singular(IScheduler scheduler) => new SingularSchedulerProvider(scheduler);

        private class DefaultSchedulerProvider : ISchedulerProvider
        {
            private DefaultSchedulerProvider()
            {
            }

            public static DefaultSchedulerProvider Instance { get; } = new DefaultSchedulerProvider();

            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Outer not used")]
            public IScheduler Default => ThreadPool;

            public IScheduler CurrentThread => CurrentThreadScheduler.Instance;

            //public IScheduler Dispatcher => Dispatcher
            public IScheduler Immediate => ImmediateScheduler.Instance;
            public IScheduler NewThread => NewThreadScheduler.Default;
            public IScheduler ThreadPool => ThreadPoolScheduler.Instance;
        }

        [SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible", Justification = "Dispatches to single (private) field")]
        private class SingularSchedulerProvider : ISchedulerProvider
        {
            private IScheduler SingularScheduler { get; }

            public SingularSchedulerProvider(IScheduler singularScheduler)
            {
                SingularScheduler = singularScheduler;
            }

            public IScheduler Default => SingularScheduler;

            public IScheduler CurrentThread => SingularScheduler;

            public IScheduler Immediate => SingularScheduler;

            public IScheduler NewThread => SingularScheduler;

            public IScheduler ThreadPool => SingularScheduler;
        }
    }
}