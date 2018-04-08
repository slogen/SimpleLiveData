using Scm.DataAccess.Qbservable;
using Scm.DataStorage.Efc2;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage
{
    public class AppEfc2UnitOfWork : DbContextUnitOfWork<AppEfc2Context>, ISomeUnitOfWork
    {
        public AppEfc2UnitOfWork(AppEfc2Context context) : base(context) { }

        public IMeet<A> A => Repository<A>().ToMeet();
    }
}