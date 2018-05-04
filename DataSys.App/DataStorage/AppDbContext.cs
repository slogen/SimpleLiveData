using DataSys.App.DataModel;
using Microsoft.EntityFrameworkCore;

namespace DataSys.App.DataStorage
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Installation> Installations { get; set; }
        public DbSet<Signal> Signal { get; set; }
        public DbSet<Data> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var mb = modelBuilder;
            mb.Entity<Data>().HasKey(d => new {d.InstallationId, d.SignalId, d.TimeStamp});
            mb.Entity<Installation>().Ignore(x => x.InstallationPeriod);
        }
    }
}