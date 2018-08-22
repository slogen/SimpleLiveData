using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.Presentation.Mvc.Support;
using Microsoft.AspNetCore.Mvc;

namespace DataSys.App.Presentation.Mvc
{
    [Route(RoutePrefix)]
    // TODO: Find a way to avoid clashing with the OData Controller
    public class SignalXController : AppUnitOfWorkAbstractEntityController<Signal>
    {
        public new const string RoutePrefix = "api/signal";

        public SignalXController(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}