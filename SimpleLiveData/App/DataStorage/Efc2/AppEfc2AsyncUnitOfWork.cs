using Scm.DataAccess.Qbservable;
using Scm.DataStorage.Efc2;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage.Efc2
{
    public class AppEfc2AsyncUnitOfWork : DbContextAsyncUnitOfWork<AppEfc2Context>, ISomeAsyncUnitOfWork
    {
        public AppEfc2AsyncUnitOfWork(AppEfc2Context context) : base(context)
        {
        }

        public IMeet<A> A => Repository<A>().ToMeet();
    }
}