using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public static class ClockExtensions
    {
        private static TimeSpan TimeTo(this IClock clock, DateTime time)
            => time - clock.Now();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task DelayTo(this IClock clock, DateTime delayTo, CancellationToken cancellationToken)
        {
            var span = clock.TimeTo(delayTo);
            if (span > TimeSpan.Zero)
                await clock.Delay(span, cancellationToken).ConfigureAwait(false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SleepTo(this IClock clock, DateTime delayTo)
        {
            var span = clock.TimeTo(delayTo);
            if (span > TimeSpan.Zero)
                clock.Sleep(span);
        }

        public static async Task<TResult> ScheduleAsync<TResult>(
            this IClock clock, TimeSpan span, Func<Task<TResult>> f,
            CancellationToken cancellationToken)
        {
            await clock.Delay(span, cancellationToken).ConfigureAwait(false);
            return await f().ConfigureAwait(false);
        }
        public static async Task ScheduleAsync(
            this IClock clock, TimeSpan span, Func<Task> f,
            CancellationToken cancellationToken)
        {
            await clock.Delay(span, cancellationToken).ConfigureAwait(false);
            await f().ConfigureAwait(false);
        }

        public static IClock Speedup(this IClock clock, double factor, TimeSpan? offset = null)
            => new RelativeSpeedClock(clock, factor, offset == null ? default(DateTime?) : clock.Now() + offset.Value);
    }
}