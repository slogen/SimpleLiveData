using System.Reactive.Subjects;
using Scm.DataAccess;

namespace Scm.DataStorage.Subject
{
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
}