using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleLiveData.App.DataAccess;

namespace SimpleLiveData.App.Presentation.Owin
{
    [RoutePrefix("/api")]
    public class QueryController
    {
        QueryExecutor QueryExecutor;
        static TimeSpan DefaultTimeOut = TimeSpan.FromSeconds(1);
        public async Task<IResult<AFilterOnStrResponse>> AFilterOnStr(
            AFilterOnStrQuery query,
            CancellationToken cancellationToken, 
            TimeSpan? timeout = null)
            => await QueryExecutor.RelevantAStuffForQuery(query)
                .TakeUntil(Observable.Timer(timeout ?? DefaultTimeOut))
                .ToResult(cancellationToken);
    }
}