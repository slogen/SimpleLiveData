using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataAccess.Qbservable;

namespace Scm.DataStorage.Subject
{
    /// <summary>
    /// Implements <see cref="IMeet{TEntity}"/> based on an <see cref="ISubject{T}"/>
    /// </summary>
    public abstract class AbstractSubjectMeet<TEntity> : IMeet<TEntity>
    {
        protected abstract ISubject<TEntity> Subject { get; }

        public virtual IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities,
            IScheduler scheduler = null)
            where TSource : TEntity
            => entities.Select(x => (TEntity) x).Do(Subject.OnNext).Scan(0L, (a, x) => a + 1).PublishLast();

        public virtual TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
            => f(Subject.AsQbservable());
    }
}