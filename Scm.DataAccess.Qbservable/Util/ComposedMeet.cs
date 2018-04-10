using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ComposedMeet<TEntity> : IMeet<TEntity>
    {
        public ComposedMeet(IObservableSource<TEntity> source, IObservableSink<TEntity> sink)
        {
            Source = source;
            Sink = sink;
        }

        public IObservableSource<TEntity> Source { get; }
        public IObservableSink<TEntity> Sink { get; }

        public virtual IObservable<TResult> Observe<TResult>(Func<IQbservable<TEntity>, IObservable<TResult>> f)
            => Source.Observe(f);

        public virtual IObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity
            => Sink.Add(entities, scheduler);
    }
}