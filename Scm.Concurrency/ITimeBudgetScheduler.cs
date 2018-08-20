using System;
using System.Runtime.CompilerServices;

namespace Scm.Concurrency
{
    public interface ITimeBudgetScheduler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TimeSpan NextSchedule(int cost);
    }
}