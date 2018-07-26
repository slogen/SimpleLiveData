using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Scm.DataAccess.Efc2;
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

        protected struct EntityChange
        {
            public EntityState State { get; }
            public object Entity { get; }
            public EntityChange(EntityState state, object entity)
            {
                State = state;
                Entity = entity;
            }
        }

        protected virtual IEnumerable<EntityEntry> FindAndOrderChanges()
            => DbContext.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached);
        protected virtual async Task PushChanges(IEnumerable<EntityEntry> changes, CancellationToken cancellationToken)
        {
            await PushChanges(changes
                // push all deletes first
                .OrderBy(x => x.State != EntityState.Deleted)
                // then modified
                .ThenBy(x => x.State != EntityState.Modified)
                // leaving inserts last
                .ToObservable(Scheduler))
                .LastOrDefaultAsync()
                .ToTask(cancellationToken)
                .ConfigureAwait(false);
        }
        protected virtual IObservable<IEntityEvent<object>> PushChanges(IObservable<EntityEntry> changes)
        => changes
                .GroupBy(x => x.Entity.GetType())
                .Select(grp =>
                    SubjectContext.Sink(grp.Key)
                        .DynamicChange(grp.GroupBy(x => x.State.ToEntityChange(), x => x.Entity)))
                .Concat();

        protected override async Task CommitAsyncOnce(CancellationToken cancellationToken)
        {
            // Capture changes before saving, so we can push them
            var changes = FindAndOrderChanges().ToList();
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await PushChanges(changes, cancellationToken).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DbContext.Dispose();
        }

        public IPersistentEntity<TEntity> Persistent<TEntity>() where TEntity : class =>
            DbContext.Repository().Of<TEntity>();

        public IQbservable<IChange<TEntity>> Live<TEntity>() where TEntity : class => SubjectContext.Qbserve<TEntity>();
        public ISink<TEntity> Sink<TEntity>() where TEntity : class
            => DbContext.Repository().Of<TEntity>();
    }
}