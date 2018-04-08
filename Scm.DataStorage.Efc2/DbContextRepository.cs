using Microsoft.EntityFrameworkCore;

namespace Scm.DataStorage.Efc2
{
    public class DbContextRepository<TContext, TEntity> : AbstractDbContextRepository<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        protected override TContext Context { get; }
        public DbContextRepository(TContext context) { Context = context; }
    }
}
