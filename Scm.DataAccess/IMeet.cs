namespace Scm.DataAccess
{
    public interface IMeet : ISink
    {
    }

    public interface IMeet<TEntity> : ILiveEntity<TEntity>, IMeet
        where TEntity : class
    {
    }
}