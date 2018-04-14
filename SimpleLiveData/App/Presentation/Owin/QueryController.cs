

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Scm.DataAccess.Qbservable;
using Scm.Rx;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.Owin
{
    [Route("api/MyEntity")]
    public class QueryController: Controller
    {
        public static TimeSpan DefaultTimeSpan = TimeSpan.FromSeconds(5);
        public QueryController() { }

        private QueryController(ISomeAsyncUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public ISomeAsyncUnitOfWork UnitOfWork { get; }
        public static TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(1);

        // TODO: Move to share
        protected IQueryable<T> Apply<T>(IObservableSource<T> src, ODataQueryOptions<T> options,
            Func<IQbservable<T>, IObservable<T>> f)
        {
            var appliedOptions = src.Observe(q => f(options.ApplyTo(q.ToQueryable()).Cast<T>().ToQbservable()));
            return appliedOptions.AsQbservable().ToQueryable();
        }

        protected IQueryable<T> Apply<T>(IObservableSource<T> src, ODataQueryOptions<T> options, TimeSpan? timeSpan,
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

        [Route("A")]
        [HttpGet]
        [HttpPost]
        public
             IEnumerable<MyEntity>
            Foo(
             //ODataQueryOptions<MyEntity> options, TimeSpan? timeSpan = null,
             CancellationToken cancellationToken = default(CancellationToken))
        {
            return Enumerable.Range(0, 3).Select(x => new MyEntity(Guid.NewGuid(), x.ToString()));
            //return Apply(UnitOfWork.A, options, timeSpan, cancellationToken);
        }
    }
}