using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Scm.DataAccess.Qbservable
{
    public interface IObservableSource<TEntity>
    {
        IQbservable<TResult> Observe<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            IScheduler scheduler = null);
    }
}
