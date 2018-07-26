using Microsoft.EntityFrameworkCore;

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
}