using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataAccess.Qbservable;

namespace Scm.DataStorage.Subject
{
    public abstract class AbstractSubjectMeet<TEntity> : IMeet<TEntity>
    {
        protected abstract ISubject<TEntity> Subject { get; }
        public IObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity
            => entities.Select(x => (TEntity)x).Do(Subject.OnNext).Scan(0L, (a, x) => a + 1).PublishLast();

        // TODO: Use IQbservable
        public IQbservable<TResult> Observe<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null, IScheduler scheduler = null)
        {
            var q = Subject.AsQbservable();
            if ( predicate != null )
                q = q.Where(predicate);
            return q.Select(selector);
        }
    }
}
