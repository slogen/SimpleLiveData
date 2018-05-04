using System;
using Microsoft.EntityFrameworkCore;

namespace Scm.DataAccess.Efc2
{
    public static class RepositoryExtensions
    {
        public static DbSetRepository<TContext> Repository<TContext>(this TContext context)
            where TContext : DbContext => new DbSetRepository<TContext>(context);

        public class DbSetRepository<TEntity, TContext> : AbstractDbContextRepository<TEntity>
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

        public class DbSetRepository<TContext> where TContext : DbContext
        {
            public DbSetRepository(TContext context)
            {
                Context = context;
            }

            public TContext Context { get; }

            public IRepository<TEntity> Of<TEntity>() where TEntity : class
                => new DbSetRepository<TEntity, TContext>(Context);

            public IRepository Of(Type entityType) =>
                (IRepository) Activator.CreateInstance(
                    typeof(DbSetRepository<,>).MakeGenericType(entityType, typeof(TContext)), Context);
        }
    }
}