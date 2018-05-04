namespace Scm.DataAccess
{
    public interface IModifyEntityEvent<out TEntity> : IEntityEvent<TEntity>
        where TEntity : class
    {
    }
}