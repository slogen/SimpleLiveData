using System;
using System.Reactive.Linq;

namespace Scm.DataAccess
{
    public interface IQbservableSource<out TEntity>
    {
        TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f);
    }
}