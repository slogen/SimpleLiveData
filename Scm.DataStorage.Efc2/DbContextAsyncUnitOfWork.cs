using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Queryable;

namespace Scm.DataStorage.Efc2
{
    public class DbContextAsyncUnitOfWork<TDbContext> : AbstractContextAsyncUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        // Same repository for everyone accessing this context
        protected ConcurrentDictionary<Type, IRepository> Repositories = new ConcurrentDictionary<Type, IRepository>();

        public DbContextAsyncUnitOfWork(TDbContext context)
        {
            Context = context;
        }

        protected override TDbContext Context { get; }

        protected IRepository<TEntity> Repository<TEntity>()
            where TEntity : class
            => (IRepository<TEntity>) Repositories.GetOrAdd(typeof(TEntity), t =>
                new DbSetRepository<TEntity>(Context.Set<TEntity>()));
    }
}