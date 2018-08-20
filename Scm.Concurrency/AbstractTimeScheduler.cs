using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public abstract class AbstractTimeScheduler : IActionScheduler
    {
        public abstract IClock Clock { get; }
        public abstract ITimeBudgetScheduler TimeBudgetScheduler { get; }

        private static bool SideEffectExceptionReturnsFalse(Action a)
        {
            try
            {
                a();
            }
            catch
            {
                // This code only evaluates a side-effect. What that throws is irrellevant
            }

            return false;
        }

        public IAsyncResult BeginSchedule<T>(int estimateCost, Func<IAsyncResult> begin, Func<IAsyncResult, T> endFunction, Func<T, int> actualCost = null, Func<Exception, int> exceptionCost = null)
            => ScheduleAsync(estimateCost, () => Task<T>.Factory.FromAsync(begin(), endFunction), default(CancellationToken), actualCost, exceptionCost);
        public T EndSchedule<T>(IAsyncResult result) => ((Task<T>) result).Result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Schedule<TResult>(int estimateCost, Func<TResult> action, Func<TResult, int> actualCost, Func<Exception, int> exceptionalCost = null)
        {
            var slp = TimeBudgetScheduler.NextSchedule(estimateCost);
            Clock.Sleep(slp);
            TResult result;
            try
            {
                result = action();
            }
            catch (Exception ex) when (exceptionalCost != null
                && SideEffectExceptionReturnsFalse(() => TimeBudgetScheduler.NextSchedule(exceptionalCost(ex) - estimateCost)))
            {
                throw; // Will not happen
            }
            if (actualCost != null)
                TimeBudgetScheduler.NextSchedule(actualCost(result) - estimateCost);
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<TResult> ScheduleAsync<TResult>(int estimateCost, Func<Task<TResult>> action, CancellationToken cancellationToken, Func<TResult, int> actualCost = null, Func<Exception, int> exceptionalCost = null)
        {
            var slp = TimeBudgetScheduler.NextSchedule(estimateCost);
            TResult result;
            try
            {
                result = await Clock.ScheduleAsync(slp, action, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (exceptionalCost != null
                && SideEffectExceptionReturnsFalse(() => TimeBudgetScheduler.NextSchedule(exceptionalCost(ex) - estimateCost)))
            {
                throw; // Will not happen
            }
            if (actualCost != null)
                TimeBudgetScheduler.NextSchedule(actualCost(result) - estimateCost);
            return result;
        }
    }
}