using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace Scm.DataAccess.Qbservable
{
    public interface IObservableSink<in TEntity>
    {
        IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity;
    }
}