using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.DataAccess.Qbservable;

namespace Scm.DataStorage.Subject
{
    public class NoDataMeet
    {
        public static NoDataMeet<TEntity> Empty<TEntity>() => NoDataMeet<TEntity>.Default;
        public static NoDataMeet<TEntity> Empty<TEntity>(TEntity witness) => Empty<TEntity>();
    }

    /// <summary>
    /// A meet that does not generate or consume data
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class NoDataMeet<TEntity> : IMeet<TEntity>
    {
        private NoDataMeet()
        {
        }

        public static NoDataMeet<TEntity> Default => new NoDataMeet<TEntity>();

        public TResult Observe<TResult>(Func<IQbservable<TEntity>, TResult> f)
            => f(Observable.Empty<TEntity>().AsQbservable());

        public IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : TEntity
            => Observable.Empty<long>().Publish(0);
    }
}