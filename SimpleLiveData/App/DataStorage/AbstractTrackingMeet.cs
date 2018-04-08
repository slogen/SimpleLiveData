using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Scm.DataAccess.Qbservable;

namespace SimpleLiveData.App.DataStorage
{
    public abstract class AbstractTrackingMeet<TEntity, TTrack> : ITrackingMeet<TEntity>
        where TTrack: ICollection<TEntity>
    {
        protected abstract IMeet<TEntity> Source { get; }
        public abstract TTrack Track { get; }

        ICollection<TEntity> ITracking<TEntity>.Track => Track;

        protected virtual IObservable<T> CheckDisposed<T>(IObservable<T> check) => check;
        protected virtual IQbservable<T> CheckDisposed<T>(IQbservable<T> check) => check;
        public IObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null) where TSource : TEntity
            => Source.Add(CheckDisposed(entities).Do(e => Track.Add(e)), scheduler);

        public IQbservable<TResult> Observe<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, IScheduler scheduler = null)
            => CheckDisposed(Source.Observe(selector, predicate, scheduler));
    }
}