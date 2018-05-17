using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess;

namespace DataSys.App.Presentation.Plain.Support
{
    [Route(RoutePrefix)]
    public abstract class EntityController<TEntity, TResult> : Controller
        where TEntity : AbstractEntity
    {
        public const string RoutePrefix = "api/[controller]";
        protected abstract IQueryableSource<TEntity> Source { get; }
        protected abstract ISink<TEntity> Sink { get; }
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
            var saved = await Sink.Add(Observable.Return(entity)).Count();
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
}