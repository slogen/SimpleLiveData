using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Efc2;
using Scm.DataAccess.Rx;
using Scm.DataAccess.Support;
using Scm.DataStorage.Efc2;
using Scm.DataStorage.Subject;

namespace Scm.DataAccess.Combined
{
    public abstract class CombinedUnitOfWork<TDbContext, TSubjectContext> : AbstractSingleCommitAsyncUnitOfWork
        where TDbContext : DbContext
        where TSubjectContext : SubjectContext
    {
        public abstract TDbContext DbContext { get; }
        public abstract TSubjectContext SubjectContext { get; }
        public virtual IScheduler Scheduler { get; } = TaskPoolScheduler.Default;

        protected override async Task CommitAsyncOnce(CancellationToken cancellationToken)
        {
            var changes = DbContext.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .OrderBy(x => x.State != EntityState.Deleted)
                .ThenBy(x => x.State != EntityState.Modified)
                .ToList();

            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await changes.ToObservable(Scheduler)
                .GroupBy(x => x.Entity.GetType())
                .Select(grp =>
                    SubjectContext.Sink(grp.Key).DynamicChange(grp.Select(e => e.ToChange()).GroupBy(x => x.Change)))
                .Concat()
                .LastOrDefaultAsync()
                .ToTask(cancellationToken)
                .ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DbContext.Dispose();
        }

        public IPersistentEntity<TEntity> Persistent<TEntity>() where TEntity : class =>
            DbContext.Repository().Of<TEntity>();

        public ILiveEntity<TEntity> Live<TEntity>() where TEntity : class => SubjectContext.Meet().Of<TEntity>();
        public ISink<TEntity> Sink<TEntity>() where TEntity : class => Persistent<TEntity>();
    }
}