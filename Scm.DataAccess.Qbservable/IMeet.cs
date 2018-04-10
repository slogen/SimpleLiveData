namespace Scm.DataAccess.Qbservable
{
    public interface IMeet<TEntity> : IObservableSource<TEntity>, IObservableSink<TEntity>
    {
    }
}