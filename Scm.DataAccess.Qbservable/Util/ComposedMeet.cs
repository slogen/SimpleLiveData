using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ComposedMeet<TEntity> : IMeet<TEntity>
    {
        public IObservableSource<TEntity> Source { get; }
        public IObservableSink<TEntity> Sink { get; }

        public ComposedMeet(IObservableSource<TEntity> source, IObservableSink<TEntity> sink)
        {
            Source = source;
            Sink = sink;
        }

        public IQbservable<TResult> Observe<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null, IScheduler scheduler = null)
            => Source.Observe(selector, predicate, scheduler);

        public IObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity
            => Sink.Add(entities, scheduler);
    }
}