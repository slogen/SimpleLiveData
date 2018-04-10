using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ObservableSourceFromQueryableSource<TEntity> : AbstractObservableSourceFromQueryableSource<TEntity>
    {
        public ObservableSourceFromQueryableSource(IQueryableSource<TEntity> source)
        {
            Source = source;
        }

        public override IQueryableSource<TEntity> Source { get; }
    }
}