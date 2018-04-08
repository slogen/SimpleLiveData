namespace Scm.DataAccess.Queryable
{

    public interface IRepository<TEntity>: IQueryableSource<TEntity>, IEnumerableAsyncSink<TEntity>
    {
    }
}
