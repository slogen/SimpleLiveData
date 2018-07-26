namespace Scm.DataAccess
{
    public interface IRepository
    {
        // TODO: add non-generic stuff?
    }

    public interface IRepository<TEntity> : IPersistentEntity<TEntity>, ISink<TEntity>
        where TEntity : class
    {
    }
}