using Scm.Presentation.OData;
using SimpleLiveData.App.DataAccess;

namespace SimpleLiveData.App.Presentation.OData
{
    public abstract class DataUnitOfWorkControllerBase<TEntity> : UnitOfWorkODataControllerBase<IDataUnitOfWork, TEntity>
        where TEntity : class
    {
        protected override IODataOptions ODataOptions { get; }

        protected DataUnitOfWorkControllerBase(IDataUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork)
        {
            ODataOptions = oDataOptions;
        }
    }
}