using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess.Qbservable;
using Scm.DataAccess.Queryable;
using Scm.Sys;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.Plain
{
    [Route("api/[controller]")]
    public abstract class EntityController<TEntity, TResult> : Controller
        where TEntity: AbstractEntity
    {
        protected abstract IQueryableSource<TEntity> Source { get; }
        protected abstract IObservableSink<TEntity> Sink { get; }
        protected abstract TResult ToProtocol(TEntity entity);
        protected abstract TEntity FromProtocol(TResult item);

        protected virtual IEnumerable<TResult> ToProtocol(IEnumerable<TEntity> entities)
            => entities.Select(ToProtocol);
        protected virtual IAsyncEnumerable<TResult> ToProtocol(IAsyncEnumerable<TEntity> entities)
            => entities.Select(ToProtocol);

        [HttpGet("{id}")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TResult>> Get(Guid id, CancellationToken cancellationToken)
        {
            var entity = await Source.Query(es => es.FirstOrDefaultAsync(cancellationToken)).ConfigureAwait(false);
            if (entity == null)
                return NotFound(id);
            return Ok(ToProtocol(entity));
        }
        [HttpPut("")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TResult>> Put(TResult newItem, CancellationToken cancellationToken)
        {
            var entity = FromProtocol(newItem);
            var saved = await Sink.Add(Observable.Return(entity).LastOrDefaultAsync());
            if (saved <= 0)
                return NotFound(entity.Id);
            return Ok(entity.Id); // TODO: Link to created object
        }

        [HttpGet("offset={offset},count={count}")]
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<IAsyncEnumerable<TResult>> List(int offset, int count, CancellationToken cancellationToken)
        {
            return Ok(Source.Query(es => es.Skip(offset).Take(count).ToAsyncEnumerable()));
        }

        [HttpGet("all")]
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<IAsyncEnumerable<TResult>> List(CancellationToken cancellationToken)
            => List(0, int.MaxValue, cancellationToken);

    }
    [Route("Installation")]
    public class InstallationController: EntityController<Installation, DataSys.Protocol.Installation>
    {
        public IDataUnitOfWork UnitOfWork { get; }

        public InstallationController(IDataUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected override IQueryableSource<Installation> Source => UnitOfWork.Installations;
        protected override IObservableSink<Installation> Sink => throw new NotImplementedException();

        protected override DataSys.Protocol.Installation ToProtocol(Installation entity)
            => new DataSys.Protocol.Installation(entity.Id, entity.Name, entity.InstallationPeriod.From, entity.InstallationPeriod.To);

        protected override Installation FromProtocol(DataSys.Protocol.Installation item)
        {
            return new Installation(item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, item.Name, new Period(item.From, item.To));
        }
    }
}
