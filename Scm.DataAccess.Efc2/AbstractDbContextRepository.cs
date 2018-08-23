using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.Linq;

namespace Scm.DataAccess.Efc2
{
    public abstract class AbstractDbContextRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected abstract DbSet<TEntity> Set { get; }


        public IQueryable<TEntity> Queryable => Set;
        public void Add(TEntity entity) => Set.Add(entity);
        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
            => await Set.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
            => await Task.Factory.StartNew(() => Set.RemoveRange(entities), cancellationToken).ConfigureAwait(false);
    }

    public abstract class AbstractDbContextRepository<TId, TEntity> : AbstractDbContextRepository<TEntity>, IIdRepository<TId, TEntity>
        where TEntity : class
    {
        public abstract Expression<Func<TEntity, TId>> IdExpression { get; }

        public async Task<TEntity> ByIdAsync(TId id, CancellationToken cancellationToken)
            => await Set.SingleOrDefaultAsync(IdExpression.Before(F.Eq(id)), cancellationToken).ConfigureAwait(false);
    }

}