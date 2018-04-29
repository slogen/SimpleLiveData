namespace Scm.DataAccess
{
    public interface IChange<out TEntity> : IChange
    {
        new TEntity Entity { get; }
    }

    public interface IChange
    {
        EntityChange Change { get; }
        object Entity { get; }
    }
}