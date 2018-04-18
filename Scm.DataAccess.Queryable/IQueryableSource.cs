using System;
using System.Linq;

namespace Scm.DataAccess.Queryable
{
    public interface IQueryableSource<out TEntity>
    {
        //IQueryable Query(Func<IQueryable, IQueryable> f);
        /// <summary>
        /// Access repository of <see cref="TEntity"/>, applying f to its content
        /// </summary>
        TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> f);
    }
}