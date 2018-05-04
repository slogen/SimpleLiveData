namespace Scm.DataAccess
{
    public abstract class AbstractDeleteEntityEvent<TEntity> : AbstractEntityEvent<TEntity>, IAddEntityEvent<TEntity>
        where TEntity : class
    {
        public override EntityChange Change => EntityChange.Delete;
    }
}