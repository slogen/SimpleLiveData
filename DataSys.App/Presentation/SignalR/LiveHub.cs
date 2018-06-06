using System;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.SignalR;
using Scm.DataAccess;

namespace DataSys.App.Presentation.SignalR
{
    public class LiveHub : Hub
    {
        public LiveHub(IAppUnitOfWork source)
        {
            Source = source;
        }

        public IAppUnitOfWork Source { get; }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    Source.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public string Hello() => "World!";

        public class Sequenced<T> {
            public long Index { get; }
            public T Value { get; }
        }

        public IObservable<IData> Observe(ODataQueryOptions<IData> queryOptions = null)
        {
            // TODO: Apply query options
            var src = Source.Live<Data>();
            IQbservableSource<Data> srcq = src;
            var obs = srcq.Observe();
            return obs;
        }
    }
}