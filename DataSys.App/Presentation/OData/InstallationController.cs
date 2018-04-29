using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.Presentation.OData.Support;
using Scm.DataAccess;
using Scm.Presentation.OData;

namespace DataSys.App.Presentation.OData
{
    public class InstallationController : DataUnitOfWorkControllerBase<Installation>
    {
        public InstallationController(IAppUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork,
            oDataOptions)
        {
        }

        protected override IQueryableSource<Installation> GetSource()
            => UnitOfWork.Persistent<Installation>();
    }
}