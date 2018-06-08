namespace Scm.DataAccess
{
    public interface IMeet : ISink
    {
    }

    public interface IMeet<out TEntity> : ILiveEntity<TEntity>, IMeet
        where TEntity : class
    {
    }
}