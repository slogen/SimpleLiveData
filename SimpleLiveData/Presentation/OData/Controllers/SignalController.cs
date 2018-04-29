using Scm.DataAccess.Queryable;
using Scm.Presentation.OData;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;
using SimpleLiveData.App.Presentation.OData.Controllers.Support;

namespace SimpleLiveData.App.Presentation.OData.Controllers
{
    public class SignalController : DataUnitOfWorkControllerBase<Signal>
    {
        public SignalController(IDataUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork, oDataOptions)
        {
        }

        protected override IQueryableSource<Signal> GetSource()
            => UnitOfWork.Signals;
    }
}