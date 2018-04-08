using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Rx
{
    public static class AsyncFuncObservableExtensions
    {
        public static IObservable<TResult> ToObservable<TResult>(this Func<CancellationToken, Task<TResult>> asyncFunc, IScheduler scheduler)
        {
            return scheduler == null ? System.Reactive.Linq.Observable.FromAsync(asyncFunc) : System.Reactive.Linq.Observable.FromAsync(asyncFunc, scheduler);
        }
    }
}
