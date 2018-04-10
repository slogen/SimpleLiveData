using Microsoft.EntityFrameworkCore;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage.Efc2
{
    public class AppEfc2Context : DbContext
    {
        public DbSet<A> A { get; set; }
    }
}