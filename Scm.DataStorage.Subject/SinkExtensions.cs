using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataAccess;
using Scm.Rx;

namespace Scm.DataStorage.Subject
{
    public static class SinkExtensions
    {
        public static SubjectSink<TContext> Sink<TContext>(this TContext context)
            where TContext : SubjectContext => new SubjectSink<TContext>(context);

        public abstract class AbstractSubjectSink<TEntity> : ISink<TEntity>
            where TEntity : class
        {
            protected abstract ISubject<IChange<TEntity>> Subject { get; }

            public IObservable<IEntityEvent<TEntity>> Change(
                IObservable<IGroupedObservable<EntityChange, TEntity>> change)
            {
                return change.Select(grp => grp.Select(e =>
                    {
                        Subject.OnNext(DataAccess.Change.Create(grp.Key, e));
                        return EntityEvent.From(grp.Key, e);
                    }))
                    .Merge();
            }

            public IObservable<IEntityEvent<object>> DynamicChange(
                IObservable<IGroupedObservable<EntityChange, object>> change)
                => Change(change.SelectGrouped(grp => grp.Cast<TEntity>())).Cast<IEntityEvent<object>>();

            protected class EntityEvent : AbstractEntityEvent<TEntity>, IEntityEvent<object>
            {
                public EntityEvent(EntityChange change, TEntity entity)
                {
                    Change = change;
                    Entity = entity;
                }

                public override TEntity Entity { get; }

                public override EntityChange Change { get; }
                object IEntityEvent<object>.Entity => Entity;
                public static EntityEvent From(EntityChange change, TEntity entity) => new EntityEvent(change, entity);
            }
        }

        public class SubjectSink<TEntity, TContext> : AbstractSubjectSink<TEntity>
            where TContext : SubjectContext
            where TEntity : class
        {
            public SubjectSink(TContext context)
            {
                Context = context;
            }

            protected TContext Context { get; }
            protected override ISubject<IChange<TEntity>> Subject => Context.Subject<TEntity>();
        }

        public class SubjectSink<TContext> where TContext : SubjectContext
        {
            public SubjectSink(TContext context)
            {
                Context = context;
            }

            public TContext Context { get; }

            public ISink<TEntity> Of<TEntity>() where TEntity : class
                => new SubjectSink<TEntity, TContext>(Context);

            public ISink Of(Type entityType) =>
                (ISink) Activator.CreateInstance(typeof(SubjectSink<,>).MakeGenericType(entityType, typeof(TContext)),
                    Context);
        }
    }
}