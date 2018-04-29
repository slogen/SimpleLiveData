using System;
using System.Linq;

namespace Scm.DataAccess
{
    public interface IQueryableSource<out TEntity>
    {
        TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> f);
    }
}