using System;
using Microsoft.EntityFrameworkCore;

namespace Scm.DataAccess.Efc2
{
    public static class DbSetRepositoryExtensions
    {
        public static DbSetRepository<TContext> Repository<TContext>(this TContext context)
            where TContext : DbContext => new DbSetRepository<TContext>(context);

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