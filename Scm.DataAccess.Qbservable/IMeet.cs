namespace Scm.DataAccess.Qbservable
{
    public interface IMeet<TEntity> : IQbservableSource<TEntity>, IObservableSink<TEntity>
    {
    }
}