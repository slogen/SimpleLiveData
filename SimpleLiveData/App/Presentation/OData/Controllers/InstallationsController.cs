using Scm.DataAccess.Queryable;
using Scm.Presentation.OData;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.OData.Controller
{
    public class InstallationsController: DataUnitOfWorkControllerBase<Installation>
    {
        public InstallationsController(IDataUnitOfWork unitOfWork, IODataOptions oDataOptions): base(unitOfWork, oDataOptions) { }
        protected override IQueryableSource<Installation> GetSource()
            => UnitOfWork.Installations;
    }
}