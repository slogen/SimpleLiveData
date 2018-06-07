using System;
using System.Reactive.Linq;
using System.Threading;

namespace Scm.Rx
{
    public static class ObserveSemaphoreSlim
    {
        public static IObservable<long> ObserveRelease(this SemaphoreSlim semaphore)
        {
            return Observable.Create<long>(async (obs, ct) =>
            {
                var i = 0L;
                while (!ct.IsCancellationRequested)
                {
                    await semaphore.WaitAsync(ct).ConfigureAwait(false);
                    obs.OnNext(i++);
                }
            });
        }
    }
}