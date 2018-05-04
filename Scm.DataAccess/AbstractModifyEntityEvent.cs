namespace Scm.DataAccess
{
    public abstract class AbstractModifyEntityEvent<TEntity> : AbstractEntityEvent<TEntity>, IAddEntityEvent<TEntity>
        where TEntity : class
    {
        public override EntityChange Change => EntityChange.Modify;
    }
}