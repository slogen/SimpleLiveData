using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Scm.DataAccess.Efc2
{
    public static class QueryableObserveExtensions
    {
        public static IObservable<T> ToObservable<T>(IQueryable<T> query)
            => query is IAsyncEnumerable<T> isAsync ? isAsync.ToObservable() : query.ToAsyncEnumerable().ToObservable();

        public static IObservable<T> ToObservable<T>(this IAsyncEnumerable<T> query)
            => Observable.Using(query.GetEnumerator,
                it => Observable.Create<T>(async (obs, ct) =>
                {
                    try
                    {
                        while (await it.MoveNext(ct).ConfigureAwait(false)) obs.OnNext(it.Current);
                    }
                    catch (Exception ex) when (!ct.IsCancellationRequested)
                    {
                        obs.OnError(ex);
                    }
                    finally

                    {
                        obs.OnCompleted();
                    }
                }));
    }
}