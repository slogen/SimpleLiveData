using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Scm.DataAccess.Efc2
{
    public class DbContextAsyncUnitOfWork<TDbContext> : AbstractDbContextAsyncUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        public DbContextAsyncUnitOfWork(TDbContext context)
        {
            Context = context;
        }

        protected override TDbContext Context { get; }

        protected IRepository<TEntity> Repository<TEntity>()
            where TEntity : class => Context.Repository().Of<TEntity>();
        protected IIdRepository<TId, TEntity> Repository<TId, TEntity>(Expression<Func<TEntity, TId>> idExpression)
            where TEntity : class => Context.Repository().Of(idExpression);
    }
}