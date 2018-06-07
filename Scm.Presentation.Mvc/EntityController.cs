using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess;

namespace Scm.Presentation.Mvc
{
    public abstract class EntityController<TEntity, TResult> : EntityController<Guid, TEntity, TResult> where TEntity : class
    {
    }

    // TODO: Move to library
    [Route(RoutePrefix)]
    public abstract class EntityController<TId, TEntity, TResult> : ControllerBase
        where TEntity: class
    {
        public const string RoutePrefix = "api/[controller]";
        protected abstract IQueryableSource<TEntity> Source { get; }
        protected abstract ISink<TEntity> Sink { get; }
        protected abstract TResult ToProtocol(TEntity entity);
        protected abstract TEntity FromProtocol(TResult item);
        protected abstract Task SaveChanges(CancellationToken cancellationToken);
        protected abstract Expression<Func<TEntity, TId>> IdExpression { get; }
        protected abstract TId Id(TEntity entity);

        protected virtual IEnumerable<TResult> ToProtocol(IEnumerable<TEntity> entities)
            => entities.Select(ToProtocol);

        protected virtual IEnumerable<TResult> ToProtocol(IAsyncEnumerable<TEntity> entities)
            => entities.Select(ToProtocol).ToEnumerable();

        [HttpGet("{id}")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TResult>> Get(TId id, CancellationToken cancellationToken)
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
            var id = Id(entity);
            var added = await Sink.Add(Observable.Return(entity)).Count();
            if (added <= 0)
                return NotFound(id);
            await SaveChanges(cancellationToken).ConfigureAwait(false);
            return Ok(id); // TODO: Link to created object
        }

        [HttpGet("offset={offset},count={count}")]
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<TResult>> List(int offset, int count, CancellationToken cancellationToken)
        {
            return Ok(ToProtocol(Source.Query(es => es.OrderBy(IdExpression).Skip(offset).Take(count).ToAsyncEnumerable())));
        }

        [HttpGet("all")]
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<TResult>> List(CancellationToken cancellationToken)
            => List(0, int.MaxValue, cancellationToken);
    }
}