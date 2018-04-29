using System;
using System.Reactive.Linq;

namespace Scm.DataAccess
{
    public interface ISink
    {
        IObservable<long> DynamicChange(IObservable<IGroupedObservable<EntityChange, object>> change);
    }

    public interface ISink<in TEntity> : ISink
    {
        IObservable<long> Change(IObservable<IGroupedObservable<EntityChange, TEntity>> change);
    }
}