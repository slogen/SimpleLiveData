using System;
using System.Threading;
using System.Threading.Tasks;
using Scm.DataAccess;

namespace Scm.Presentation.Mvc
{
    public abstract class
        UnitOfWorkEntityController<TUnitOfWork, TEntity, TResult> : UnitOfWorkEntityController<TUnitOfWork, Guid, TEntity, TResult> 
        where TUnitOfWork : IAsyncUnitOfWork 
        where TEntity : class
    {
        protected UnitOfWorkEntityController(TUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
    public abstract class UnitOfWorkEntityController<TUnitOfWork, TId, TEntity, TResult>: EntityController<TId, TEntity, TResult>, IDisposable
        where TUnitOfWork: IAsyncUnitOfWork
        where TEntity: class
    {
        public UnitOfWorkEntityController(TUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public TUnitOfWork UnitOfWork { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            UnitOfWork.Dispose();
        }

        ~UnitOfWorkEntityController()
        {
            Dispose(false);
        }

        protected override async Task SaveChanges(CancellationToken cancellationToken)
            => await UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}