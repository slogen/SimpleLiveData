using DataSys.App.DataAccess;
using Scm.Presentation.OData;

namespace DataSys.App.Presentation.OData.Support
{
    public abstract class
        DataUnitOfWorkControllerBase<TEntity> : UnitOfWorkODataControllerBase<IDataUnitOfWork, TEntity>
        where TEntity : class
    {
        protected DataUnitOfWorkControllerBase(IDataUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(
            unitOfWork)
        {
            ODataOptions = oDataOptions;
        }

        protected override IODataOptions ODataOptions { get; }
    }
}