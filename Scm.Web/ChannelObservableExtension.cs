using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace Scm.Web
{
    public static class ChannelObservableExtension
    {
        public static ReadyMonitoredObservable<T> Observe<T>(this HubConnection hubConnection, string methodName)
            => hubConnection.Observe<T>(methodName, args: null);

        /// <summary>
        /// Subscribe to remote <paramref name="methodName"/>(<paramref name="args"/>).
        /// 
        /// Returned task will complete when the channel is stablished.
        /// </summary>
        /// <remarks>The returned observable is shared with Publish().Refcount()</remarks>
        public static ReadyMonitoredObservable<T> Observe<T>(this HubConnection hubConnection, string methodName,
            object[] args)
        {
            var ready = new BehaviorSubject<int>(0);
            var subscribeCount = 0;
            var observable = Observable.Create<T>(async (obs, ct) =>
                {
                    var reader = await hubConnection
                        .StreamAsChannelAsync<T>(methodName, args ?? new object[0], ct).ConfigureAwait(false);
                    var id = Interlocked.Increment(ref subscribeCount);
                    ready.OnNext(id);
                    try
                    {
                        while (await reader.WaitToReadAsync(ct).ConfigureAwait(false))
                        while (reader.TryRead(out T item))
                            obs.OnNext(item);
                    }
                    catch (Exception ex) when (!ct.IsCancellationRequested)
                    {
                        obs.OnError(ex);
                        ready.OnError(ex);
                    }
                    finally
                    {
                        ready.OnNext(-id);
                    }

                    if (!ct.IsCancellationRequested)
                        obs.OnCompleted();
                })
                .Publish().RefCount()
                .Finally(() => ready.OnCompleted());
            return new ReadyMonitoredObservable<T>(observable, ready);
        }

        public class ReadyMonitoredObservable<T> : IObservable<T>
        {
            public ReadyMonitoredObservable(IObservable<T> observable, IObservable<int> ready)
            {
                Observable = observable;
                Ready = ready;
            }

            public IObservable<T> Observable { get; }
            public IObservable<int> Ready { get; }

            public IDisposable Subscribe(IObserver<T> observer) => Observable.Subscribe(observer);
        }
    }
}