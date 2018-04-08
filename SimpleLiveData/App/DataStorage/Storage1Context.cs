using Microsoft.EntityFrameworkCore;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage
{

    public class Storage1Context: DbContext
    {
        public DbSet<A> A { get; set; }
        public DbSet<B> B { get; set; }
    }
}
