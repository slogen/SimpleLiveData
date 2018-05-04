namespace Scm.DataAccess
{
    public interface IEntityEvent<out TEntity>
        where TEntity : class
    {
        EntityChange Change { get; }
        TEntity Entity { get; }
    }
}