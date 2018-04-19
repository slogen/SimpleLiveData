using Scm.Presentation.OData;
using SimpleLiveData.App.DataAccess;

namespace SimpleLiveData.App.Presentation.OData.Controllers.Support
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