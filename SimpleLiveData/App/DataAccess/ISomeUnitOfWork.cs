using Scm.DataAccess;
using Scm.DataAccess.Qbservable;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataAccess
{
    public interface ISomeUnitOfWork: IUnitOfWork
    {
        IMeet <A> A { get; }
    }
}
