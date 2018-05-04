namespace Scm.DataAccess
{
    public abstract class AbstractAddEntityEvent<TEntity> : AbstractEntityEvent<TEntity>, IAddEntityEvent<TEntity>
        where TEntity : class
    {
        public override EntityChange Change => EntityChange.Add;
    }
}