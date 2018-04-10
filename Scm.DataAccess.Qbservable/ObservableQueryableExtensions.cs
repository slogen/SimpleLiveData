using System;
using Scm.DataAccess.Qbservable.Util;
using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable
{
    public static class ObservableQueryableExtensions
    {
        public static IObservableSource<TEntity> ToQbservableSource<TEntity>(
            this IQueryableSource<TEntity> source)
            where TEntity : class
            => new ObservableSourceFromQueryableSource<TEntity>(source);

        public static IObservableSink<TEntity> ToObservableSink<TEntity>(
            this IEnumerableAsyncSink<TEntity> sink,
            TimeSpan? chunkTimeSpan = null,
            int? chunkSize = null)
            where TEntity : class
            => new ObservableSinkFromEnumerableAsyncSink<TEntity>(sink, chunkTimeSpan, chunkSize);

        public static IMeet<TEntity> ToMeet<TEntity>(
            this IRepository<TEntity> repo,
            TimeSpan? chunkTimeSpan = null,
            int? chunkSize = null,
            bool? commitBeforeObserve = null)
            where TEntity : class
        {
            IMeet<TEntity> meet = new ComposedMeet<TEntity>(
                repo.ToQbservableSource(),
                repo.ToObservableSink(chunkTimeSpan, chunkSize));
            return meet;
        }
    }
}