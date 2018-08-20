using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public interface IClock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        DateTime Now();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Task Delay(TimeSpan span, CancellationToken cancellationToken);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Sleep(TimeSpan span);
    }
}