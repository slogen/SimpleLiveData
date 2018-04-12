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