namespace Scm.DataAccess.Queryable
{
    public interface IRepository
    {
        // TODO: add non-generic stuff?
    }

    public interface IRepository<TEntity> : IQueryableSource<TEntity>, ISink<TEntity>, IRepository
    {
    }
}