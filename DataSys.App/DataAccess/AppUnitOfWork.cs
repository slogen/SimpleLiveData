using System;
using DataSys.App.DataModel;
using DataSys.App.DataStorage;
using Scm.DataAccess;
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
        public IIdRepository<Guid, Installation> Installations => IdRepository((Installation x) => x.Id);
        public IIdRepository<Guid, Signal> Signals => IdRepository((Signal x) => x.Id);
    }
}