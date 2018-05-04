using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Scm.Rx;

namespace Scm.DataAccess.Efc2
{
    public abstract class AbstractDbContextRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected abstract DbSet<TEntity> Set { get; }

        public TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> f)
            => f(Set);

        public TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
            => Query(q => f(q.ToQbservable()));

        public IObservable<IEntityEvent<TEntity>> Change(IObservable<IGroupedObservable<EntityChange, TEntity>> changes)
        {
            var obs = changes.Select(grp =>
            {
                switch (grp.Key)
                {
                    case EntityChange.Add:
                        return grp.Select(entity => AddEvent.From(Set.Add(entity)));
                    case EntityChange.Modify:
                        return grp.Select(entity => ModifyEvent.From(Set.Update(entity)));
                    case EntityChange.Delete:
                        return grp.Select(entity => DeleteEvent.From(Set.Remove(entity)));
                    default:
                        throw new NotSupportedException($"Cannot {grp.Key} on {GetType()}");
                }
            }).Merge();
            return obs;
        }

        public IObservable<IEntityEvent<object>> DynamicChange(
            IObservable<IGroupedObservable<EntityChange, object>> changes)
            => Change(changes.GroupedSelect(grp => grp.Cast<TEntity>())).Cast<IEntityEvent<object>>();

        protected abstract class AbstractEntityEntryEvent : AbstractEntityEvent<TEntity>, IEntityEvent<object>
        {
            protected AbstractEntityEntryEvent(EntityEntry<TEntity> entityEntry)
            {
                EntityEntry = entityEntry;
            }

            public EntityEntry<TEntity> EntityEntry { get; }
            public override TEntity Entity => EntityEntry.Entity;

            object IEntityEvent<object>.Entity => Entity;
        }

        protected class AddEvent : AbstractEntityEntryEvent
        {
            public AddEvent(EntityEntry<TEntity> entityEntry) : base(entityEntry)
            {
            }

            public override EntityChange Change => EntityChange.Add;
            public static IEntityEvent<TEntity> From(EntityEntry<TEntity> entityEntry) => new AddEvent(entityEntry);
        }

        protected class ModifyEvent : AbstractEntityEntryEvent
        {
            public ModifyEvent(EntityEntry<TEntity> entityEntry) : base(entityEntry)
            {
            }

            public override EntityChange Change => EntityChange.Modify;
            public static IEntityEvent<TEntity> From(EntityEntry<TEntity> entityEntry) => new ModifyEvent(entityEntry);
        }

        protected class DeleteEvent : AbstractEntityEntryEvent
        {
            public DeleteEvent(EntityEntry<TEntity> entityEntry) : base(entityEntry)
            {
            }

            public override EntityChange Change => EntityChange.Delete;
            public static IEntityEvent<TEntity> From(EntityEntry<TEntity> entityEntry) => new DeleteEvent(entityEntry);
        }
    }
}