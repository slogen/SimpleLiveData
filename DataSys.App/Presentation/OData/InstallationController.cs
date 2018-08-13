using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.Presentation.OData.Support;
using Scm.Presentation.OData;
using System.Linq;

namespace DataSys.App.Presentation.OData
{
    public class InstallationController : DataUnitOfWorkControllerBase<Installation>
    {
        public InstallationController(IAppUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork,
            oDataOptions)
        {
        }

        protected override IQueryable<Installation> Source
            => UnitOfWork.Persistent<Installation>();
    }
}