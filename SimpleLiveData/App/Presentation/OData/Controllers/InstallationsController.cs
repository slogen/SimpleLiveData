using Scm.DataAccess.Queryable;
using Scm.Presentation.OData;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;
using SimpleLiveData.App.Presentation.OData.Controllers.Support;

namespace SimpleLiveData.App.Presentation.OData.Controllers
{
    public class InstallationsController: DataUnitOfWorkControllerBase<Installation>
    {
        public InstallationsController(IDataUnitOfWork unitOfWork, IODataOptions oDataOptions): base(unitOfWork, oDataOptions) { }
        protected override IQueryableSource<Installation> GetSource()
            => UnitOfWork.Installations;
    }
}