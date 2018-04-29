using Scm.DataAccess;

namespace DataSys.App.DataAccess
{
    public interface IAppUnitOfWork : IAsyncUnitOfWork
    {
        IPersistentEntity<TEntity> Persistent<TEntity>() where TEntity : class;
        ILiveEntity<TEntity> Live<TEntity>() where TEntity : class;
        ISink<TEntity> Sink<TEntity>() where TEntity : class;
    }
}