using DataSys.App.DataModel;
using Microsoft.EntityFrameworkCore;

namespace DataSys.App.DataStorage
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<Installation> Installations { get; set; }
        public DbSet<Signal> Signal { get; set; }
        public DbSet<Data> Data { get; set; }
    }
}