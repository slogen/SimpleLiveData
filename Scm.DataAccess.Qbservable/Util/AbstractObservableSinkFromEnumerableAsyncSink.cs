using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataAccess.Queryable;
using Scm.Rx;

namespace Scm.DataAccess.Qbservable.Util
{
    public abstract class AbstractObservableSinkFromEnumerableAsyncSink<TEntity> : IObservableSink<TEntity>
        where TEntity : class
    {
        public abstract IEnumerableAsyncSink<TEntity> Sink { get; }
        public abstract int? ChunkSize { get; }
        public abstract TimeSpan? TimeSpan { get; }

        public IConnectableObservable<long> Add<TSource>(
            IObservable<TSource> entities,
            IScheduler scheduler = null)
            where TSource : TEntity
            => entities.Buffer(TimeSpan, ChunkSize, scheduler)
                .ScanAsync(0L, async (acc, next, ct) =>
                {
                    if (next.Count > 0)
                        await Sink.AddRangeAsync(next, ct).ConfigureAwait(false);
                    return acc + next.Count;
                })
                .Publish(0);
    }
}