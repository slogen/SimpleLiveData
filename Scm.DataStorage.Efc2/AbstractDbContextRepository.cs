using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Queryable;

namespace Scm.DataStorage.Efc2
{
    public abstract class AbstractDbContextRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected abstract DbSet<TEntity> Set { get; }

        public async Task AddRangeAsync<TSource>(IEnumerable<TSource> source, CancellationToken cancellationToken)
            where TSource : TEntity
            => await Set.AddRangeAsync(source.Cast<TEntity>(), cancellationToken).ConfigureAwait(false);

        public TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> f)
            => f(Set);
    }
}