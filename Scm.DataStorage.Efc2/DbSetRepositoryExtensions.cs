using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Queryable;

namespace Scm.DataStorage.Efc2
{
    public static class DbSetRepositoryExtensions
    {
        public static IRepository<TEntity> ToRepository<TEntity>(this DbSet<TEntity> set)
            where TEntity: class
            => new DbSetRepository<TEntity>(set);
    }
}