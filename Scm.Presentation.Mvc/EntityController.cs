using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scm.DataAccess;

namespace Scm.Presentation.Mvc
{
    public abstract class EntityController<TEntity, TResult> : EntityController<IIdRepository<Guid, TEntity>, Guid, TEntity, TResult>
        where TEntity : class
    { }
    public abstract class EntityController<TId, TEntity, TProtocol> : EntityController<IIdRepository<TId, TEntity>, TId, TEntity, TProtocol>
        where TEntity : class
    {
    }
    [Route(RoutePrefix)]
    public abstract class EntityController<TRepository, TId, TEntity, TProtocol> : ControllerBase
        where TEntity : class
        where TRepository: IIdRepository<TId, TEntity>
    {
        public const string RoutePrefix = "api/[controller]";
        protected abstract TRepository Source { get; }
        protected abstract TProtocol ToProtocol(TEntity entity);
        protected virtual TEntity FromProtocol(TProtocol item) => throw new NotImplementedException();
        protected virtual void Copy(TEntity src, TEntity dest) => throw new NotImplementedException();
        protected abstract Task SaveChanges(CancellationToken cancellationToken);
        protected abstract TId Id(TEntity entity);
 
        protected virtual IEnumerable<TProtocol> ToProtocol(IEnumerable<TEntity> entities)
            => entities.Select(ToProtocol);

        protected virtual IEnumerable<TProtocol> ToProtocol(IAsyncEnumerable<TEntity> entities)
            => entities.Select(ToProtocol).ToEnumerable();

        [HttpGet("{id}")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public virtual async Task<ActionResult<TProtocol>> Get(TId id, CancellationToken cancellationToken)
        {
            var entity = await Source.ByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (entity == null)
                return NotFound(id);
            return Ok(ToProtocol(entity));
        }

        [HttpPut("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public virtual async Task<ActionResult<TProtocol>> Put(TProtocol newItem, CancellationToken cancellationToken)
        {
            var entity = FromProtocol(newItem);
            var id = Id(entity);
            var added = await Source.Add(Observable.Return(entity)).Count();
            if (added <= 0)
                return NotFound(id);
            await SaveChanges(cancellationToken).ConfigureAwait(false);
            return Ok(id);
        }
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public virtual async Task<ActionResult<TProtocol>> Patch(TId id, TProtocol newItem, CancellationToken cancellationToken)
        {
            var oldEntity = await Source.ByIdAsync(id, cancellationToken);
            if (oldEntity == null)
                return NotFound(id);
            var newEntity = FromProtocol(newItem);
            Copy(newEntity, oldEntity);
            await SaveChanges(cancellationToken).ConfigureAwait(false);
            return Ok(oldEntity);
        }
        [HttpGet("ids/offset={offset},count={count}")]
        [HttpPost]
        [ProducesResponseType(200)]
        public virtual ActionResult<IEnumerable<TId>> IdList(int? offset, int? count, CancellationToken cancellationToken)
        {
            var q = Source.Queryable.Select(Source.IdExpression).OrderBy(x => x).AsQueryable();
            if (offset.HasValue)
                q = q.Skip(offset.Value);
            if (count.HasValue)
                q = q.Take(count.Value);
            return Ok(q.ToAsyncEnumerable());
        }

        [HttpGet("ids/all")]
        [HttpPost]
        [ProducesResponseType(200)]
        public virtual ActionResult<IEnumerable<TId>> IdList(CancellationToken cancellationToken)
            => IdList(null, null, cancellationToken);

        [HttpGet("offset={offset},count={count}")]
        [HttpPost]
        [ProducesResponseType(200)]
        public virtual ActionResult<IEnumerable<TProtocol>> List(int? offset, int? count,
            CancellationToken cancellationToken)
        {
            var q = Source.Queryable.OrderBy(Source.IdExpression).AsQueryable();
            if (offset.HasValue)
                q = q.Skip(offset.Value);
            if (count.HasValue)
                q = q.Take(count.Value);
            return Ok(ToProtocol(q.ToAsyncEnumerable()));
        }

        public const string AllRoute = "all";
        [HttpGet(AllRoute)]
        [HttpPost]
        [ProducesResponseType(200)]
        public virtual ActionResult<IEnumerable<TProtocol>> List(CancellationToken cancellationToken)
            => List(null, null, cancellationToken);
    }
}