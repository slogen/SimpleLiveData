using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Scm.DataAccess.Qbservable
{
    public abstract class AbstractInstantMeet<TEntity> : IMeet<TEntity>
    {
        protected abstract IMeet<TEntity> Inner { get; }
        protected abstract ISubject<TEntity> InstantSubject { get; }
        protected IQbservable<TEntity> Instant => InstantSubject.AsQbservable();

        public IQbservable<TResult> Observe<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null, IScheduler scheduler = null)
        {
            var historic = Inner.Observe(selector, predicate, scheduler);
            var live = Instant;
            if (predicate != null)
                live = live.Where(predicate);
            return historic.Merge(live.Select(selector), scheduler);
        }

        public IObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity
            => Inner.Add(entities.Do(x => InstantSubject.OnNext(x)), scheduler);
    }
}