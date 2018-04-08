using System;
using System.Linq;
using System.Linq.Expressions;

namespace Scm.DataAccess.Queryable
{
    public interface IQueryableSource<TEntity>
    {
        /// <summary>
        /// Access repository of <see cref="TEntity"/>, applying <paramref name="selector"/> to each entity,
        /// optionally filtering for <paramref name="filter"/>
        /// </summary>
        /// <remarks>This allows building of arbitrary expressions efficiently</remarks>
        IQueryable<TResult> Query<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null);
    }
}
