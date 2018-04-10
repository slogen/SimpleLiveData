using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess.Queryable
{
    public interface IEnumerableAsyncSink<in TEntity>
    {
        Task AddRangeAsync<TSource>(IEnumerable<TSource> source, CancellationToken cancellationToken)
            where TSource : TEntity;
    }
}