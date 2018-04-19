using System.Reactive.Concurrency;

namespace Scm.Rx
{
    public static class SchedulerProviderExtensions
    {
        public static IScheduler Default(this ISchedulerProvider provider)
            => provider?.Default ?? SchedulerProvider.DefaultInstance.Default;

        public static IScheduler CurrentThread(this ISchedulerProvider provider)
            => provider?.CurrentThread ?? SchedulerProvider.DefaultInstance.CurrentThread;

        public static IScheduler Immediate(this ISchedulerProvider provider)
            => provider?.Immediate ?? SchedulerProvider.DefaultInstance.Immediate;

        public static IScheduler NewThread(this ISchedulerProvider provider)
            => provider?.NewThread ?? SchedulerProvider.DefaultInstance.NewThread;

        public static IScheduler ThreadPool(this ISchedulerProvider provider)
            => provider?.ThreadPool ?? SchedulerProvider.DefaultInstance.ThreadPool;
    }
}