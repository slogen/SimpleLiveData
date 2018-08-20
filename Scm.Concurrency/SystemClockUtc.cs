using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public class SystemClockUtc : IClock
    {
        public static SystemClockUtc Default { get; set; } = new SystemClockUtc();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime Now() => DateTime.UtcNow;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task Delay(TimeSpan span, CancellationToken cancellationToken)
        {
            if (span > TimeSpan.Zero)
                await Task.Delay(span, cancellationToken).ConfigureAwait(false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sleep(TimeSpan span)
        {
            if ( span > TimeSpan.Zero )
                Thread.Sleep(span);
        }
    }
}