using System;
using System.Linq;

namespace Scm.DataAccess.Queryable
{
    public interface IQueryableSource<out TEntity>
    {
        /// <summary>
        /// Access repository of <see cref="TEntity"/>, applying f to its content
        /// </summary>
        IObservable<TResult> Query<TResult>(Func<IQueryable<TEntity>, IObservable<TResult>> f);
    }
}