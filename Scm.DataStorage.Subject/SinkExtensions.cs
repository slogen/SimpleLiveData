using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataAccess;
using Scm.Rx;

namespace Scm.DataStorage.Subject
{
    public static class SinkExtensions
    {
        public abstract class AbstractSubjectSink<TEntity> : ISink<TEntity>
        {
            protected abstract ISubject<IChange<TEntity>> Subject { get; }

            public IObservable<long> Change(IObservable<IGroupedObservable<EntityChange, TEntity>> change)
            {
                return change.Select(grp => grp.Select(e =>
                    {
                        Subject.OnNext(DataAccess.Change.Create(grp.Key, e));
                        return e;
                    }))
                    .Merge().HotCount();
            }

            public IObservable<long> DynamicChange(IObservable<IGroupedObservable<EntityChange, object>> change)
                => Change(change.SelectGrouped(grp => grp.Cast<TEntity>()));

        }
        public class SubjectSink<TEntity, TContext> : AbstractSubjectSink<TEntity>
            where TContext : SubjectContext
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
            public ISink<TEntity> Of<TEntity>() where TEntity: class
                => new SubjectSink<TEntity, TContext>(Context);

            public IMeet Of(Type entityType) =>
                (IMeet) Activator.CreateInstance(typeof(SubjectSink<,>).MakeGenericType(entityType, typeof(TContext)), Context);
        }
        public static SubjectSink<TContext> Sink<TContext>(this TContext context)
            where TContext: SubjectContext => new SubjectSink<TContext>(context);
    }
}