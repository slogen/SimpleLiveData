namespace Scm.DataAccess
{
    public interface IPersistentEntity<TEntity> : IQueryableSource<TEntity>
        where TEntity : class
    {
    }
}