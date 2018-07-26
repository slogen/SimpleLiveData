using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Scm.DataAccess;

namespace Scm.Presentation.OData
{
    public abstract class AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TEntity>
        : AbstractUnitOfWorkODataControllerBase<TUnitOfWork, IQueryable<TEntity>, TEntity>
        where TUnitOfWork : class, IAsyncUnitOfWork
    {
    }

    public abstract class AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TSource, TEntity> : ODataController
        where TUnitOfWork : class, IAsyncUnitOfWork
        where TSource : IQueryable<TEntity>
    {
        private TUnitOfWork _unitOfWork;
        protected TUnitOfWork UnitOfWork => _unitOfWork ?? (_unitOfWork = GetUnitOfWork());
        protected abstract TSource Source { get; }

        protected virtual IODataOptions ODataOptions => null;
        protected abstract TUnitOfWork GetUnitOfWork();

        public virtual IQueryable Get(ODataQueryOptions<TEntity> queryOptions)
        {
            var ao = ODataOptions.ApplyOptions();
            return queryOptions.ApplyTo(Source, ao.QuerySettings(), ao.IgnoredQueryOptions());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _unitOfWork?.Dispose();
        }

        protected void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AbstractUnitOfWorkODataControllerBase()
        {
            Dispose(false);
        }
    }
}