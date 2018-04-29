using System;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.Presentation.Plain.Support;
using Microsoft.AspNetCore.Mvc;
using Scm.DataAccess.Qbservable;
using Scm.DataAccess.Queryable;
using Scm.Sys;

namespace DataSys.App.Presentation.Plain
{
    [Route("api/Installation")]
    // TODO: Find a way to avoid clashing with the OData Controller
    public class InstallationXController : EntityController<Installation, Protocol.Installation>
    {
        public InstallationXController(IDataUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IDataUnitOfWork UnitOfWork { get; }

        protected override IQueryableSource<Installation> Source => UnitOfWork.Installations;
        protected override IObservableSink<Installation> Sink => throw new NotImplementedException();

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