using System;
using System.Reactive.Linq;

namespace Scm.DataAccess.Qbservable
{
    public interface IObservableSource<out TEntity>
    {
        IObservable<TResult> Observe<TResult>(Func<IQbservable<TEntity>, IObservable<TResult>> f);
    }
}