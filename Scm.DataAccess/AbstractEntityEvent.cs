namespace Scm.DataAccess
{
    public abstract class AbstractEntityEvent<TEntity> : IEntityEvent<TEntity>
        where TEntity : class
    {
        public abstract EntityChange Change { get; }
        public abstract TEntity Entity { get; }
    }
}