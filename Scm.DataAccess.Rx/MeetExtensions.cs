using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataStorage.Subject;

namespace Scm.DataAccess.Rx
{
    public static class MeetExtensions
    {
        public static SubjectMeet<TContext> Meet<TContext>(this TContext context)
            where TContext : SubjectContext => new SubjectMeet<TContext>(context);

        public class SubjectMeet<TEntity, TContext> : DataStorage.Subject.SinkExtensions.AbstractSubjectSink<TEntity>, IMeet<TEntity>
            where TContext : SubjectContext
        {
            public SubjectMeet(TContext context)
            {
                Context = context;
            }

            protected TContext Context { get; }
            protected override ISubject<IChange<TEntity>> Subject => Context.Subject<TEntity>();

            public TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
                => Observe(q => f(q.Where(x => x.Change != EntityChange.Delete).Select(x => x.Entity)));

            public TResult Observe<TResult>(Func<IQbservable<IChange<TEntity>>, TResult> f)
                => f(Subject.AsQbservable());
        }

        public class SubjectMeet<TContext> where TContext : SubjectContext
        {
            public SubjectMeet(TContext context)
            {
                Context = context;
            }

            public TContext Context { get; }

            public IMeet<TEntity> Of<TEntity>() where TEntity : class
                => new SubjectMeet<TEntity, TContext>(Context);

            public IMeet Of(Type entityType) =>
                (IMeet) Activator.CreateInstance(typeof(SubjectMeet<,>).MakeGenericType(entityType, typeof(TContext)),
                    Context);
        }
    }
}