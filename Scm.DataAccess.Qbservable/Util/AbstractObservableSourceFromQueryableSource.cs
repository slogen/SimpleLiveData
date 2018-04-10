using System;
using System.Reactive.Linq;
using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public abstract class AbstractObservableSourceFromQueryableSource<TEntity> : IObservableSource<TEntity>
    {
        public abstract IQueryableSource<TEntity> Source { get; }

        public IObservable<TResult> Observe<TResult>(Func<IQbservable<TEntity>, IObservable<TResult>> f)
            => Source.Query(q => f(q.ToQbservable()));
    }
}