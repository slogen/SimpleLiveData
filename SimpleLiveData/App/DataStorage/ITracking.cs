using System.Collections.Generic;

namespace SimpleLiveData.App.DataStorage
{
    public interface ITracking<TEntity>
    {
        ICollection<TEntity> Track { get; }
    }
}