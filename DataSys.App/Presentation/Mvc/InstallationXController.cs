using System;
using DataSys.App.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Scm.DataAccess;
using Scm.Sys;

namespace DataSys.App.Presentation.Mvc
{
    [Route(RoutePrefix)]
    // TODO: Find a way to avoid clashing with the OData Controller
    public class
        InstallationXController : UnitOfWorkAbstractEntityController<IAppUnitOfWork, DataModel.Installation, Protocol.Installation>
    {
        public new const string RoutePrefix = "api/installation";

        public InstallationXController(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override IQueryableSource<DataModel.Installation> Source => UnitOfWork.Persistent<DataModel.Installation>();
        protected override ISink<DataModel.Installation> Sink => throw new NotImplementedException();

        protected override Protocol.Installation ToProtocol(DataModel.Installation entity)
            => new Protocol.Installation(entity.Id, entity.Name, entity.InstallationPeriod.From,
                entity.InstallationPeriod.To);

        protected override DataModel.Installation FromProtocol(Protocol.Installation item)
        {
            return new DataModel.Installation(item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, item.Name,
                new Period(item.From, item.To));
        }
    }
}