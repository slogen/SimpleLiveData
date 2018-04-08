using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Queryable;

namespace Scm.DataStorage.Efc2
{
    public abstract class AbstractDbContextRepository<TEntity>: IRepository<TEntity>
        where TEntity: class
    {
        protected abstract DbSet<TEntity> Set { get; }

        public async Task AddRangeAsync<TSource>(IEnumerable<TSource> source, CancellationToken cancellationToken)
            where TSource: TEntity
            => await Set.AddRangeAsync(source.Cast<TEntity>(), cancellationToken).ConfigureAwait(false);

        public IQueryable<TResult> Query<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> q = Set;
            if (filter != null)
                q = q.Where(filter);
            return q.Select(selector);
        }
    }
}
