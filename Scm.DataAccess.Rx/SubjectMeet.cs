using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataStorage.Subject;

namespace Scm.DataAccess.Rx
{
    public class SubjectMeet<TEntity, TContext> : AbstractSubjectSink<TEntity>,
        IMeet<TEntity>
        where TContext : SubjectContext
        where TEntity : class
    {
        public SubjectMeet(TContext context)
        {
            Context = context;
        }

        protected TContext Context { get; }
        protected override ISubject<IChange<TEntity>> Subject => Context.Subject<TEntity>();

        public TResult Observe<TResult>(Func<IQbservable<IChange<TEntity>>, TResult> f)
            => f(Subject.AsQbservable());

        public TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
            => Observe(q => f(q.Where(x => x.Change != EntityChange.Delete).Select(x => x.Entity)));
    }
}