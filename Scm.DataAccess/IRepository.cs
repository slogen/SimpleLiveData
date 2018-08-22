using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scm.Linq;

namespace Scm.DataAccess
{
    public interface IRepository
    {
        // TODO: add non-generic stuff?
    }

    public interface IRepository<TEntity> 
        where TEntity : class
    {
        IQueryable<TEntity> Queryable { get; }
        Task AddRangeAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    }

    public interface IIdRepository<TId, TEntity>: IRepository<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, TId>> IdExpression { get; }
        Task<TEntity> ByIdAsync(TId id, CancellationToken cancellationToken);
    }

    public static class RepositoryExtensions
    {
        public static TEntity ById<TId, TEntity>(this IIdRepository<TId, TEntity> repo, TId id)
            where TEntity : class
            => repo.Queryable.SingleOrDefault(repo.IdExpression.Before(F.Eq(id)));

        public static async Task AddAsync<TEntity>(this IRepository<TEntity> repo, TEntity entity,
            CancellationToken cancellationToken) where TEntity : class
            => await repo.AddRangeAsync(new[] {entity}, cancellationToken).ConfigureAwait(false);

        public static IObservable<Unit> Add<TEntity>(this IRepository<TEntity> repo, IObservable<TEntity> entities)
            where TEntity : class
            => Observable.FromAsync(ct => repo.AddRangeAsync(entities.ToEnumerable(), ct));

        public static async Task RemoveAsync<TEntity>(this IRepository<TEntity> repo, TEntity entity,
            CancellationToken cancellationToken) where TEntity : class
            => await repo.RemoveRangeAsync(new[] {entity}, cancellationToken).ConfigureAwait(false);

        public static IObservable<Unit> Remove<TEntity>(this IRepository<TEntity> repo, IObservable<TEntity> entities)
            where TEntity : class
            => Observable.FromAsync(ct => repo.RemoveRangeAsync(entities.ToEnumerable(), ct));
    }
}