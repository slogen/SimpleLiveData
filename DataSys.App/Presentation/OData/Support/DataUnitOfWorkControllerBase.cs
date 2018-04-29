using DataSys.App.DataAccess;
using Scm.Presentation.OData;

namespace DataSys.App.Presentation.OData.Support
{
    public abstract class
        DataUnitOfWorkControllerBase<TEntity> : UnitOfWorkODataControllerBase<IAppUnitOfWork, TEntity>
        where TEntity : class
    {
        protected DataUnitOfWorkControllerBase(IAppUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(
            unitOfWork)
        {
            ODataOptions = oDataOptions;
        }

        protected override IODataOptions ODataOptions { get; }
    }
}