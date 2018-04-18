using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ComposedMeet<TEntity> : IMeet<TEntity>
    {
        public ComposedMeet(IQbservableSource<TEntity> source, IObservableSink<TEntity> sink)
        {
            Source = source;
            Sink = sink;
        }

        public IQbservableSource<TEntity> Source { get; }
        public IObservableSink<TEntity> Sink { get; }

        public virtual TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
            => Source.Observe(f);

        public virtual IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity
            => Sink.Add(entities, scheduler);
    }
}