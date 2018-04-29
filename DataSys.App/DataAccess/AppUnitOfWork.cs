using DataSys.App.DataStorage;
using Scm.DataAccess.Combined;

namespace DataSys.App.DataAccess
{
    public class AppUnitOfWork : CombinedUnitOfWork<AppDbContext, AppSubjectContext>, IAppUnitOfWork
    {
        public AppUnitOfWork(AppDbContext dbContext, AppSubjectContext subjectContext)
        {
            DbContext = dbContext;
            SubjectContext = subjectContext;
        }

        public override AppDbContext DbContext { get; }
        public override AppSubjectContext SubjectContext { get; }
    }
}