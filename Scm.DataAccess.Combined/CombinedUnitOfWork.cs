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

        protected bool IncludeEntity(EntityEntry e)
            => e.State != EntityState.Unchanged && e.State != EntityState.Detached;

        protected struct OriginalState
        {
            public object Entity { get; }
            public EntityState State { get; }
            public OriginalState(object entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }
            public static OriginalState FromEntityEntry(EntityEntry entityEntry)
                => new OriginalState(entityEntry.Entity, entityEntry.State);
        }
        protected virtual IEnumerable<OriginalState> FindAndOrderChanges()
            => DbContext.ChangeTracker.Entries()
                .Where(IncludeEntity)
                // push all deletes first
                .OrderBy(x => x.State != EntityState.Deleted)
                // then modified
                .ThenBy(x => x.State != EntityState.Modified)
                .Select(OriginalState.FromEntityEntry)
                // leaving inserts last
                ;
        protected virtual async Task PushChanges(IEnumerable<OriginalState> changes, CancellationToken cancellationToken)
        {
            await PushChanges(changes.ToObservable(Scheduler), x => x.State, x => x.Entity)
                .LastOrDefaultAsync()
                .ToTask(cancellationToken)
                .ConfigureAwait(false);
        }
        protected virtual IObservable<IEntityEvent<object>> PushChanges<TSource>(IObservable<TSource> src, Func<TSource, EntityState> stateOf, Func<TSource, object> entityOf)
        => src
                .GroupBy(x => entityOf(x)?.GetType())
                .Select(grp =>
                    SubjectContext.Sink(grp.Key)
                        .DynamicChange(grp.GroupBy(x => stateOf(x).ToEntityChange(), x => entityOf(x))))
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

        public IQueryable<TEntity> Persistent<TEntity>() where TEntity : class =>
            DbContext.Repository().Of<TEntity>().Queryable;

        public IQbservable<IChange<TEntity>> Live<TEntity>() where TEntity : class => SubjectContext.Qbserve<TEntity>();
        public ISink<TEntity> Sink<TEntity>() where TEntity : class
            => DbContext.Repository().Of<TEntity>();
    }
}