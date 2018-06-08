namespace Scm.DataAccess
{
    public interface ILiveEntity
    {
    }

    public interface ILiveEntity<out TEntity> :
        ILiveEntity,
        IQbservableSource<IChange<TEntity>>
        where TEntity : class
    {
    }
}