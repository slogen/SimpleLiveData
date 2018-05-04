namespace Scm.DataAccess
{
    public interface IPersistentEntity<TEntity> : IQueryableSource<TEntity>, ISink<TEntity>, IQbservableSource<TEntity>
        where TEntity : class
    {
    }
}