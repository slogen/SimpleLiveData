using DataSys.App.DataStorage;
using Microsoft.EntityFrameworkCore;

namespace DataSys.App.DataAccess
{
    public class AppUnitOfWorkFactory : IAppUnitOfWorkFactory
    {
        private AppSubjectContext AppSubjectContext { get; } = new AppSubjectContext();

        public IAppUnitOfWork UnitOfWork()
        {
            var dbCtx = DbContext();
            var ctx = new AppUnitOfWork(dbCtx, AppSubjectContext);
            return ctx;
        }

        protected virtual string DbContextName => "AppDb";

        protected virtual AppDbContext DbContext() =>
            new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(DbContextName).Options);
    }
}