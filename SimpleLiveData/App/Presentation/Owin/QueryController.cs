using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Scm.DataAccess.Qbservable;
using Scm.Rx;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.Owin
{
    [RoutePrefix("/api/A")]
    public class QueryController
    {
        public static TimeSpan DefaultTimeSpan = TimeSpan.FromSeconds(5);

        public QueryController(ISomeAsyncUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public ISomeAsyncUnitOfWork UnitOfWork { get; }
        public static TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(1);

        // TODO: Move to share
        protected IQueryable<T> Apply<T>(IObservableSource<T> src, ODataQueryOptions options,
            Func<IQbservable<T>, IObservable<T>> f)
        {
            var appliedOptions = src.Observe(q => f(options.ApplyTo(q.ToQueryable()).Cast<T>().ToQbservable()));
            return appliedOptions.AsQbservable().ToQueryable();
        }

        protected IQueryable<T> Apply<T>(IObservableSource<T> src, ODataQueryOptions options, TimeSpan? timeSpan,
            CancellationToken cancellationToken)
        {
            return Apply(src, options, q =>
            {
                var o = q;
                if (cancellationToken.CanBeCanceled)
                    o = o.TakeUntil(cancellationToken.ToObservable());
                var ts = timeSpan ?? DefaultTimeSpan;
                if (ts >= TimeSpan.Zero)
                    o = o.TakeUntil(Observable.Interval(ts));
                return o;
            });
        }

        public IQueryable<A> Get(ODataQueryOptions options, TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Apply(UnitOfWork.A, options, timeSpan, cancellationToken);
        }
    }
}