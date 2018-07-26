using Scm.DataAccess;
using System.Linq;
using System.Reactive.Linq;

namespace DataSys.App.DataAccess
{
    public interface IAppUnitOfWork : IAsyncUnitOfWork
    {
        IQueryable<TEntity> Persistent<TEntity>() where TEntity : class;
        IQbservable<IChange<TEntity>> Live<TEntity>() where TEntity : class;
        ISink<TEntity> Sink<TEntity>() where TEntity : class;
    }
}