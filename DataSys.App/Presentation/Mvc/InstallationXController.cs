using System;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using Microsoft.AspNetCore.Mvc;
using Scm.Sys;

namespace DataSys.App.Presentation.Mvc
{
    [Route(RoutePrefix)]
    // TODO: Find a way to avoid clashing with the OData Controller
    public class
        InstallationXController : AppUnitOfWorkAbstractEntityController<Installation, Protocol.Installation>
    {
        public new const string RoutePrefix = "api/installation";

        public InstallationXController(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        protected override Protocol.Installation ToProtocol(Installation entity)
            => new Protocol.Installation(entity.Id, entity.Name, entity.InstallationPeriod.From,
                entity.InstallationPeriod.To);

        protected override Installation FromProtocol(Protocol.Installation item)
        {
            return new Installation(item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, item.Name,
                new Period(item.From?.UtcDateTime, item.To?.UtcDateTime));
        }
    }
}