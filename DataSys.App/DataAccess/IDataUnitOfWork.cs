using DataSys.App.DataModel;
using Scm.DataAccess;
using Scm.DataAccess.Qbservable;
using Scm.DataAccess.Queryable;

namespace DataSys.App.DataAccess
{
    public interface IDataUnitOfWork : IAsyncUnitOfWork
    {
        IQueryableSource<Installation> Installations { get; }
        IQueryableSource<Signal> Signals { get; }
        IQbservableSource<Data> Data { get; }
    }
}