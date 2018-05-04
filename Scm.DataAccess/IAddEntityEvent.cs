namespace Scm.DataAccess
{
    public interface IAddEntityEvent<out TEntity> : IEntityEvent<TEntity>
        where TEntity : class
    {
    }
}