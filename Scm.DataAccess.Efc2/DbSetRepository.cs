using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.Linq;

namespace Scm.DataAccess.Efc2
{
    public class DbSetRepository<TEntity, TContext> : 
        AbstractDbContextRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        public DbSetRepository(TContext context)
        {
            Context = context;
        }

        public TContext Context { get; }

        protected override DbSet<TEntity> Set => Context.Set<TEntity>();
    }

    public class DbIdSetRepository<TId, TEntity, TContext> : DbSetRepository<TEntity, TContext>, IIdRepository<TId, TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        public DbIdSetRepository(TContext context, Expression<Func<TEntity, TId>> idExpression) : base(context)
        {
            IdExpression = idExpression;
        }

        public Expression<Func<TEntity, TId>> IdExpression { get; }

        public async Task<TEntity> ByIdAsync(TId id, CancellationToken cancellationToken)
            => await Set.FirstOrDefaultAsync(IdExpression.Before(F.Eq(id))).ConfigureAwait(false);
    }
}