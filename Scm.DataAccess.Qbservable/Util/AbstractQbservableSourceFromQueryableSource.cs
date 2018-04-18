using System;
using System.Reactive.Linq;
using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public abstract class AbstractQbservableSourceFromQueryableSource<TEntity> : IQbservableSource<TEntity>
    {
        public abstract IQueryableSource<TEntity> Source { get; }

        public TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
            => Source.Query(q => f(q.ToQbservable()));
    }
}