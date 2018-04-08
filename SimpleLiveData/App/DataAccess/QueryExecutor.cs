using SimpleLiveData.App.DataStorage;
using System;
using System.Linq;
using System.Text;

namespace SimpleLiveData.App.DataAccess
{
    public class QueryExecutor
    {
        private ISomeUnitOfWork UnitOfWork { get; }
        public QueryExecutor(ISomeUnitOfWork someUnitOfWork) { UnitOfWork = someUnitOfWork; }

        public IObservable<AFilterOnStrResponse> RelevantAStuffForQuery(AFilterOnStrQuery filter)
        {
            return UnitOfWork.A.Observe(a => 
                new AFilterOnStrResponse
                {
                    A = new AFilterOnStrResponse.AData
                    {
                        Id = a.Id,
                        Str = a.Str
                    },
                    BIds = a.Bs.Select(x => x.Id).ToList()
                },
                a => a.Str.Contains(filter.StrContains));
        }
    }
}
