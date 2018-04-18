using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Scm.DataAccess.Qbservable;
using Scm.DataAccess.Queryable;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.Tests
{
    public class TestSource:
        AbstractTestSource, 
        IQueryableSource<Installation>, IQbservableSource<Installation>,
        IQueryableSource<Signal>, IQbservableSource<Signal>,
        IObservableSink<Data>,
        IDataUnitOfWork
    {
        public TimeSpan ObserveIntervalSpan { get; } = TimeSpan.FromSeconds(1);

        public TResult Observe<TResult>(Func<IQbservable<Installation>, TResult> f)
            => f(ObserveData.GroupByUntil(d => d.Installation, i => Observable.Interval(ObserveIntervalSpan))
                .SelectMany(g => g.ToList().Select(l => new Installation(g.Key) {Data = l})));
        public TResult Query<TResult>(Func<IQueryable<Installation>, TResult> f)
            => f(Installations.Values.AsQueryable());
        public TResult Observe<TResult>(Func<IQbservable<Signal>, TResult> f)
            => f(ObserveData.GroupByUntil(d => d.Signal, i => Observable.Interval(ObserveIntervalSpan))
                .SelectMany(g => g.ToList().Select(l => new Signal(g.Key) { Data = l })));
        public TResult Query<TResult>(Func<IQueryable<Signal>, TResult> f)
            => f(Signals.Values.AsQueryable());

        public IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : Data
            => entities.Do(AddData).Select((x, i) => i + 1L).Publish(0);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            // No-op
            return Task.CompletedTask;
        }

        IQueryableSource<Installation> IDataUnitOfWork.Installations => this;
        IQueryableSource<Signal> IDataUnitOfWork.Signals => this;
    }
}