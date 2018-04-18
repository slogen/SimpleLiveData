using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Rx
{
    public static class ObservableScanAsyncExtensions
    {
        /// <summary>
        /// Scan over <paramref name="source"/>, invoking <paramref name="accumulator"/> for each input
        /// one at a time.
        /// 
        /// This means, that <paramref name="accumulator"/> will be invoked when the previous <paramref name="accumulator"/>
        /// has completed.
        /// </summary>
        /// <remarks>This will queue up task-invocations if the <paramref name="source"/> produces items faster than <paramref name="accumulator"/>
        /// processes them. 
        /// 
        /// The whole calculation will abort if unsubscribed</remarks>
        public static IObservable<TAccumulate> ScanAsync<TSource, TAccumulate>(
            this IObservable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, CancellationToken, Task<TAccumulate>> accumulator)
            =>
                source.Scan(
                        Observable.Return(seed),
                        (acc, next) =>
                            Observable.FromAsync(async ct =>
                                await accumulator(await acc.ToTask(ct).ConfigureAwait(false), next, ct)))
                    .Concat();
    }
}