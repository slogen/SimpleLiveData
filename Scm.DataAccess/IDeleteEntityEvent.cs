namespace Scm.DataAccess
{
    public interface IDeleteEntityEvent<out TEntity> : IEntityEvent<TEntity>
        where TEntity : class
    {
    }
}