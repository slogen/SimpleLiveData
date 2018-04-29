using Microsoft.EntityFrameworkCore;

namespace Scm.DataAccess.Efc2
{
    public class DbContextAsyncUnitOfWork<TDbContext> : AbstractContextAsyncUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        public DbContextAsyncUnitOfWork(TDbContext context)
        {
            Context = context;
        }

        protected override TDbContext Context { get; }

        protected IRepository<TEntity> Repository<TEntity>()
            where TEntity : class => Context.Repository().Of<TEntity>();
    }
}