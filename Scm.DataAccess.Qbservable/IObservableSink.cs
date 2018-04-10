using System;
using System.Reactive.Concurrency;

namespace Scm.DataAccess.Qbservable
{
    public interface IObservableSink<in TEntity>
    {
        IObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity;
    }
}