using System;
using System.Diagnostics;
using System.Reactive.Linq;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.SignalR;

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


        public IObservable<IData> Observe(ODataQueryOptions<IData> queryOptions = null)
        {
            var obs = Source.Live<Data>().Observe((IQbservable<Data> datas) => datas.Select(data => data)
                .Do(
                    x => Debug.WriteLine(x),
                    ex => Debug.WriteLine(ex),
                    () => Debug.WriteLine("COMPLETED"))
                .Finally(
                    () => Debug.WriteLine("Finally")
                ));
            return obs;
        }
    }
}