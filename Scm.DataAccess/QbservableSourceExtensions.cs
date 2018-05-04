using System;

namespace Scm.DataAccess
{
    public static class QbservableSourceExtensions
    {
        public static IObservable<TEntity> Observe<TEntity>(this IQbservableSource<TEntity> source) =>
            source.Observe(q => q);
    }
}