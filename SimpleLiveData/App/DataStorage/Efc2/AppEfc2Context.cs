using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Qbservable;
using Scm.DataStorage.Efc2;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage
{

    public class AppEfc2Context: DbContext
    {
        public DbSet<A> A { get; set; }
        public DbSet<B> B { get; set; }
    }
}
