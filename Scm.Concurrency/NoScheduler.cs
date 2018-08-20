using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public class NoScheduler : IActionScheduler
    {
        public static NoScheduler Default = new NoScheduler();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAsyncResult BeginSchedule<T>(int estimateCost, Func<IAsyncResult> action, Func<IAsyncResult, T> endFunction, Func<T, int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => Task<T>.Factory.FromAsync(action(), endFunction);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T EndSchedule<T>(IAsyncResult result) => ((Task<T>)result).Result;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Schedule<TResult>(int estimateCost, Func<TResult> action, Func<TResult, int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => action();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<TResult> ScheduleAsync<TResult>(int estimateCost, Func<Task<TResult>> action, CancellationToken cancellationToken, Func<TResult, int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => await action().ConfigureAwait(false);

    }
}