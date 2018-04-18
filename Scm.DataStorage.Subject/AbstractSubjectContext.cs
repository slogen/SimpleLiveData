using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Scm.DataAccess.Qbservable;
using Scm.Rx;

namespace Scm.DataStorage.Subject
{
    public abstract class AbstractSubjectContext : ISubjectContext
    {
        private readonly ISubject<Unit> _commit = new Subject<Unit>();

        /// <summary>
        /// Observable that emits Commits. Throws if fails. Complete when Disposed
        /// </summary>
        public IObservable<Unit> Commits => _commit.AsObservable();

        public abstract IMeet<TEntity> Meet<TEntity>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            _commit.OnNext(Unit.Default);
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _commit.OnCompleted();
            else
                _commit?.OnError(new InvalidOperationException("Forgotten dispose"));
        }

        ~AbstractSubjectContext()
        {
            Dispose(false);
        }

        public class ContextMeet<TEntity> : AbstractSubjectMeet<TEntity>
        {
            public ContextMeet(AbstractSubjectContext parent, ISubject<TEntity> subject)
            {
                Parent = parent;
                Subject = subject;
            }

            protected AbstractSubjectContext Parent { get; }
            protected override ISubject<TEntity> Subject { get; }


            public override IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
                => base.Add(entities
                        .TakeUntil(Parent.Commits.LastOrDefaultAsync()
                            .Throw(_ => new ObjectDisposedException("SubjectContext Disposed")))
                    , scheduler);

            public override TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
                => base.Observe(q => f(q.TakeUntil(Parent.Commits.LastOrDefaultAsync())));
        }
    }
}