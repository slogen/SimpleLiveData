using System;
using DataSys.App.DataModel;
using Scm.DataAccess;
using Scm.Presentation.Mvc;

namespace DataSys.App.Presentation.Mvc.Support
{
    public abstract class
        UnitOfWorkAbstractEntityController<TUnitOfWork, TEntity, TResult> 
            : UnitOfWorkEntityController<TUnitOfWork, TEntity, TResult>
        where TUnitOfWork : IAsyncUnitOfWork
        where TEntity : AbstractEntity
    {
        protected override Guid Id(TEntity entity) => entity.Id;

        protected UnitOfWorkAbstractEntityController(TUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}