using System;
using System.Threading;
using System.Threading.Tasks;
using Scm.Concurrency;

namespace Scm.Sys
{
    public static class SchedulerExtensions
    {
        public static async Task ScheduleAsync(this IActionScheduler actionScheduler, int estimateCost, Func<Task> action, CancellationToken cancellationToken, Func<int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => await actionScheduler.ScheduleAsync(estimateCost, async () =>
            {
                await action().ConfigureAwait(false);
                return 0;
            }, cancellationToken, actualCost == null ? default(Func<int, int>) : _ => actualCost(), exceptionCost)
            .ConfigureAwait(false);
        public static void Schedule(this IActionScheduler actionScheduler, int cost, Action action, Func<int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => actionScheduler.Schedule(cost, () => { action(); return cost; }, actualCost == null ? default(Func<int, int>) : _ => actualCost(), exceptionCost);
        public static IAsyncResult BeginSchedule(this IActionScheduler actionScheduler,
            int estimateCost,
            Func<IAsyncResult> begin,
            Action<IAsyncResult> end,
            Func<int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => actionScheduler.BeginSchedule(estimateCost, begin, ia => { end(ia); return 0; }, actualCost == null ? default(Func<int, int>) : _ => actualCost(), exceptionCost);
    }
}
