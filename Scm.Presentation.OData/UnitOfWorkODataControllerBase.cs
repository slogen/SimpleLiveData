using Scm.DataAccess;
using System.Linq;

namespace Scm.Presentation.OData
{
    public abstract class UnitOfWorkODataControllerBase<TUnitOfWork, TEntity> :
        AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TEntity>
        where TUnitOfWork : class, IAsyncUnitOfWork
    {
        private readonly TUnitOfWork _unitOfWork;

        protected UnitOfWorkODataControllerBase(TUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected sealed override TUnitOfWork GetUnitOfWork() => _unitOfWork;
    }
}