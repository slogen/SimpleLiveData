using System;
using System.Reactive.Linq;
using Microsoft.AspNetCore.SignalR.Client;

namespace Scm.Web
{
    public static class ChannelObservableExtension
    {
        public static IObservable<T> Observe<T>(this HubConnection hubConnection, string methodName)
            => hubConnection.Observe<T>(methodName, args: null);

        /// <summary>
        /// Subscribe to remote <paramref name="methodName"/>(<paramref name="args"/>).
        /// 
        /// Returned task will complete when the channel is stablished.
        /// </summary>
        /// <remarks>The returned observable is shared with Publish().Refcount()</remarks>
        public static IObservable<T> Observe<T>(this HubConnection hubConnection, string methodName,
            object[] args)
        {
            var observable = Observable.Create<T>(async (obs, ct) =>
                {
                    var reader = await hubConnection
                        .StreamAsChannelAsync<T>(methodName, args ?? new object[0], ct).ConfigureAwait(false);
                    try
                    {
                        while (await reader.WaitToReadAsync(ct).ConfigureAwait(false))
                        while (reader.TryRead(out var item))
                            obs.OnNext(item);
                    }
                    catch (Exception ex) when (!ct.IsCancellationRequested)
                    {
                        obs.OnError(ex);
                    }

                    if (!ct.IsCancellationRequested)
                        obs.OnCompleted();
                })
                .Publish().RefCount();
            return observable;
        }
    }
}