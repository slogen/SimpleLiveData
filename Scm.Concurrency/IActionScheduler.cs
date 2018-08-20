using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public interface IActionScheduler {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Task<TResult> ScheduleAsync<TResult>(int estimateCost, Func<Task<TResult>> action, CancellationToken cancellationToken, Func<TResult, int> actualCost = null, Func<Exception, int> exceptionCost = null);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TResult Schedule<TResult>(int estimateCost, Func<TResult> action, Func<TResult, int> actualCost = null, Func<Exception, int> exceptionCost = null);

        IAsyncResult BeginSchedule<T>(
            int estimateCost, 
            Func<IAsyncResult> begin,
            Func<IAsyncResult, T> end,
            Func<T, int> actualCost = null, 
            Func<Exception, int> exceptionCost = null);
        T EndSchedule<T>(IAsyncResult result);
    }
}