using System;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.Presentation.OData.Support;
using Scm.Presentation.OData;
using System.Linq;

namespace DataSys.App.Presentation.OData
{
    public class SignalController : DataUnitOfWorkControllerBase<Signal>
    {
        public SignalController(IAppUnitOfWork unitOfWork, IODataOptions oDataOptions) : base(unitOfWork, oDataOptions)
        {
        }

        protected override IQueryable<Signal> Source => UnitOfWork.IdRepository<Guid, Signal>(x => x.Id).Queryable;
    }
}