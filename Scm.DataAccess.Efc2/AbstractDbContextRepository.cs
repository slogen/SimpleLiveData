using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
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

        public IObservable<long> Change(IObservable<IGroupedObservable<EntityChange, TEntity>> changes)
        {
            return changes.Select(grp =>
            {
                switch (grp.Key)
                {
                    case EntityChange.Add:
                        return grp.Do(entity => Set.Add(entity));
                    case EntityChange.Modify:
                        return grp.Do(entity => Set.Update(entity));
                    case EntityChange.Delete:
                        return grp.Do(entity => Set.Remove(entity));
                    default:
                        throw new NotSupportedException($"Cannot {grp.Key} on {GetType()}");
                }
            }).Merge().HotCount();
        }

        public IObservable<long> DynamicChange(IObservable<IGroupedObservable<EntityChange, object>> changes)
            => Change(changes.SelectGrouped(grp => grp.Cast<TEntity>()));
    }
}