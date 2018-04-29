namespace Scm.DataAccess
{
    public interface ILiveEntity
    {
    }

    public interface ILiveEntity<TEntity> : IQbservableSource<TEntity>, ISink<TEntity>, ILiveEntity, IQbservableSource<IChange<TEntity>>
    {
    }
}