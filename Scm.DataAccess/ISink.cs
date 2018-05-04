using System;
using System.Reactive.Linq;

namespace Scm.DataAccess
{
    public interface ISink
    {
        IObservable<IEntityEvent<object>> DynamicChange(IObservable<IGroupedObservable<EntityChange, object>> change);
    }

    public interface ISink<TEntity> : ISink
        where TEntity : class
    {
        IObservable<IEntityEvent<TEntity>> Change(IObservable<IGroupedObservable<EntityChange, TEntity>> change);
    }
}