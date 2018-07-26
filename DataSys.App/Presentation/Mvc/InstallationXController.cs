using System;
using System.Linq;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using Microsoft.AspNetCore.Mvc;
using Scm.DataAccess;
using Scm.Sys;

namespace DataSys.App.Presentation.Mvc
{
    [Route(RoutePrefix)]
    // TODO: Find a way to avoid clashing with the OData Controller
    public class
        InstallationXController : UnitOfWorkAbstractEntityController<IAppUnitOfWork, Installation, Protocol.Installation>
    {
        public new const string RoutePrefix = "api/installation";

        public InstallationXController(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override IQueryable<Installation> Source => UnitOfWork.Persistent<Installation>();
        protected override ISink<Installation> Sink => throw new NotImplementedException();

        protected override Protocol.Installation ToProtocol(Installation entity)
            => new Protocol.Installation(entity.Id, entity.Name, entity.InstallationPeriod.From,
                entity.InstallationPeriod.To);

        protected override Installation FromProtocol(Protocol.Installation item)
        {
            return new Installation(item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, item.Name,
                new Period(item.From, item.To));
        }
    }
}