using Scm.DataAccess.Queryable;
using Scm.Presentation.OData;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.OData.Controller
{
    public class SignalsController : DataUnitOfWorkControllerBase<Signal>
    {
        public SignalsController(IDataUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork, oDataOptions) { }

        protected override IQueryableSource<Signal> GetSource()
            => UnitOfWork.Signals;
    }
}