using System.Linq;

namespace Scm.DataAccess
{
    public static class QueryableResultExtensions
    {
        public static IQueryable<TEntity> Query<TEntity>(this IQueryableSource<TEntity> source) => source.Query(q => q);
    }
}