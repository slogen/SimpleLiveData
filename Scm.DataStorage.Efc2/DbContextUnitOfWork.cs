using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Queryable;

namespace Scm.DataStorage.Efc2
{
    public class DbContextUnitOfWork<TDbContext>: AbstractContextUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        public DbContextUnitOfWork(TDbContext context) {
            Context = context;
        }

        protected override TDbContext Context { get; }
        protected IRepository<TEntity> Repository<TEntity>()
            where TEntity: class
            => new DbContextRepository<TDbContext, TEntity>(Context);
    }
}
