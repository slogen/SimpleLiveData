using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public class QbservableSourceFromQueryableSource<TEntity> : AbstractQbservableSourceFromQueryableSource<TEntity>
    {
        public QbservableSourceFromQueryableSource(IQueryableSource<TEntity> source)
        {
            Source = source;
        }

        public override IQueryableSource<TEntity> Source { get; }
    }
}