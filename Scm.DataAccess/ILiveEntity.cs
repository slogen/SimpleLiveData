namespace Scm.DataAccess
{
    public interface ILiveEntity
    {
    }

    public interface ILiveEntity<TEntity> :
        ILiveEntity,
        IQbservableSource<IChange<TEntity>>
        where TEntity : class
    {
    }
}