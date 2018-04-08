using Scm.DataAccess.Qbservable;
using Scm.DataStorage.Efc2;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage
{
    public class Storage1UnitOfWork : DbContextUnitOfWork<Storage1Context>, ISomeUnitOfWork
    {
        public Storage1UnitOfWork(Storage1Context context) : base(context) { }

        public IMeet<A> A => Repository<A>().ToMeet();
    }
}