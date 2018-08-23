using System;
using Scm.DataAccess;
using System.Linq.Expressions;
using System.Reactive.Linq;
using DataSys.App.DataModel;

namespace DataSys.App.DataAccess
{
    public interface IAppUnitOfWork : IAsyncUnitOfWork
    {
        IIdRepository<Guid, Installation> Installations { get; }
        IIdRepository<Guid, Signal> Signals { get; }
        IIdRepository<TId, TEntity> IdRepository<TId, TEntity>(Expression<Func<TEntity, TId>> idExpression) where TEntity : class;
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IQbservable<IChange<TEntity>> Live<TEntity>() where TEntity : class;
    }

    public static class AppUnitOfWorkExtensions
    {
        public static IIdRepository<Guid, TEntity> Repository<TEntity>(this IAppUnitOfWork repo)
            where TEntity : AbstractEntity 
            => repo.IdRepository((TEntity x) => x.Id);
    }
}