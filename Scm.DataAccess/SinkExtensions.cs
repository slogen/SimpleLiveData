using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    public static class SinkExtensions
    {
        public static IObservable<TEntity> Change<TEntity>(this ISink<TEntity> sink, EntityChange change,
            IObservable<TEntity> entities)
            where TEntity : class
            => sink.Change(entities.Select(e => new {e, change}).GroupBy(x => x.change, x => x.e))
                .Select(x => x.Entity);

        public static IObservable<TEntity> Add<TEntity>(this ISink<TEntity> sink, IObservable<TEntity> entities)
            where TEntity : class
            => sink.Change(EntityChange.Add, entities);

        public static IObservable<TEntity> Delete<TEntity>(this ISink<TEntity> sink, IObservable<TEntity> entities)
            where TEntity : class
            => sink.Change(EntityChange.Delete, entities);

        public static async Task ChangeRangeAsync<TEntity>(this ISink<TEntity> sink, EntityChange change,
            IEnumerable<TEntity> entities, CancellationToken cancellationToken, IScheduler scheduler = null)
            where TEntity : class
            => await sink.Change(change, entities.ToObservable(scheduler ?? Scheduler.Default))
                .Count().ToTask(cancellationToken).ConfigureAwait(false);

        public static async Task AddRangeAsync<TEntity>(this ISink<TEntity> sink, IEnumerable<TEntity> entities,
                CancellationToken cancellationToken, IScheduler scheduler = null)
                where TEntity : class
                => await sink.ChangeRangeAsync(EntityChange.Add, entities, cancellationToken, scheduler).ConfigureAwait(false);
        public static async Task DeleteRangeAsync<TEntity>(this ISink<TEntity> sink, IEnumerable<TEntity> entities,
            CancellationToken cancellationToken, IScheduler scheduler = null)
            where TEntity : class
            => await sink.ChangeRangeAsync(EntityChange.Delete, entities, cancellationToken, scheduler).ConfigureAwait(false);

    }
}