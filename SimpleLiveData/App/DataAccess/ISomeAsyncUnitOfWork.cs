using Scm.DataAccess;
using Scm.DataAccess.Qbservable;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataAccess
{
    public interface ISomeAsyncUnitOfWork : IAsyncUnitOfWork
    {
        IMeet<MyEntity> A { get; }
    }
}