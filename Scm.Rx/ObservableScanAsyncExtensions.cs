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
        private static Func<TArg, TResult> F<TArg, TResult>(Func<TArg, TResult> f) => f;
        public static IObservable<TAccumulate> ScanAsync<TSource, TAccumulate>(
            this IObservable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, CancellationToken, Task<TAccumulate>> accumulator,
            IScheduler scheduler = null)
        =>
            source.Scan(
                System.Reactive.Linq.Observable.Return(seed),
                (acc, next) =>
                    F<CancellationToken, Task<TAccumulate>>(async ct =>
                        await accumulator(await acc.ToTask(ct).ConfigureAwait(false), next, ct).ConfigureAwait(false))
                        .ToObservable(scheduler))
                        .Concat();
    }
}
