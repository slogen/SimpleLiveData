using Scm.DataAccess.Qbservable;

namespace SimpleLiveData.App.DataStorage
{
    public interface ITrackingMeet<TEntity>: IMeet<TEntity>, ITracking<TEntity> { }
}