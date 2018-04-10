using System;
using System.Linq;
using System.Reactive.Linq;

namespace SimpleLiveData.App.DataAccess
{
    public class QueryExecutor
    {
        public QueryExecutor(ISomeAsyncUnitOfWork someAsyncUnitOfWork)
        {
            AsyncUnitOfWork = someAsyncUnitOfWork;
        }

        private ISomeAsyncUnitOfWork AsyncUnitOfWork { get; }

        public IObservable<AFilterOnStrResponse> RelevantAStuffForQuery(AFilterOnStrQuery filter)
        {
            return AsyncUnitOfWork.A.Observe(q => q
                    .Where(a => a.Str.Contains(filter.StrContains)))
                .Select(a => new AFilterOnStrResponse
                {
                    A = new AFilterOnStrResponse.AData
                    {
                        Id = a.Id,
                        Str = a.Str
                    },
                    BIds = a.Bs.Select(x => x.Id).ToList()
                });
        }
    }
}