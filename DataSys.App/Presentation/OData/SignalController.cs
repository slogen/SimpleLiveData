using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.Presentation.OData.Support;
using Scm.DataAccess;
using Scm.Presentation.OData;

namespace DataSys.App.Presentation.OData
{
    public class SignalController : DataUnitOfWorkControllerBase<Signal>
    {
        public SignalController(IAppUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork, oDataOptions)
        {
        }

        protected override IQueryableSource<Signal> GetSource()
            => UnitOfWork.Persistent<Signal>();
    }
}