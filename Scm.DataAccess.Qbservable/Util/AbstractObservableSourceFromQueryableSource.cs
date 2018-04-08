using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public abstract class AbstractObservableSourceFromQueryableSource<TEntity> : IObservableSource<TEntity>
    {
        public abstract IQueryableSource<TEntity> Source { get; }
        public IQbservable<TResult> Observe<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            IScheduler scheduler = null)
        {
            var q = Source.Query(selector, predicate);
            return ReferenceEquals(scheduler, null) ? q.ToQbservable() : q.ToQbservable(scheduler);
        }
    }
}
