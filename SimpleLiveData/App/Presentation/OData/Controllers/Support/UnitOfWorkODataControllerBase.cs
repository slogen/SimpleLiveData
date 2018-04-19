﻿using Scm.DataAccess;
using Scm.DataAccess.Queryable;

namespace SimpleLiveData.App.Presentation.OData.Controllers.Support
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
        protected abstract override IQueryableSource<TEntity> GetSource();
    }
}