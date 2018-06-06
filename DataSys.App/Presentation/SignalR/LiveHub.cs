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
        public const string Route = "/signalr/livedata";
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
        public IObservable<IChange<IData>> Observe(ODataQueryOptions<IData> queryOptions = null)
        {
            var src = Source.Live<Data>();
            var obs = src.Observe();
            return obs;
        }
    }
}