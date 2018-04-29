using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Scm.DataAccess;
using Scm.DataAccess.Queryable;
using Scm.Presentation.OData;

namespace DataSys.App.Presentation.OData.Support
{
    public abstract class AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TEntity>
        : AbstractUnitOfWorkODataControllerBase<TUnitOfWork, IQueryableSource<TEntity>, TEntity>
        where TUnitOfWork : class, IAsyncUnitOfWork
    {
    }

    public abstract class AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TSource, TEntity> : ODataController
        where TUnitOfWork : class, IAsyncUnitOfWork
        where TSource : class, IQueryableSource<TEntity>
    {
        private TSource _source;
        private TUnitOfWork _unitOfWork;
        protected TUnitOfWork UnitOfWork => _unitOfWork ?? (_unitOfWork = GetUnitOfWork());
        protected TSource Source => _source ?? (_source = GetSource());

        protected virtual IODataOptions ODataOptions => null;
        protected abstract TUnitOfWork GetUnitOfWork();
        protected abstract TSource GetSource();

        public virtual IQueryable Get(ODataQueryOptions<TEntity> queryOptions)
        {
            var ao = ODataOptions.ApplyOptions();
            var x = Source.Query(qin =>
            {
                var q1 = qin;
                var q2 = queryOptions.ApplyTo(q1, ao.QuerySettings(), ao.IgnoredQueryOptions());
                return q2;
            });
            return x;
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