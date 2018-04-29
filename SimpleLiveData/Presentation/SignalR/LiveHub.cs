using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.SignalR
{
    public class LiveHub : Hub
    {
        public LiveHub(IDataUnitOfWork source)
        {
            Source = source;
        }

        public IDataUnitOfWork Source { get; }

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
            var obs = Source.Data.Observe(datas => datas.Select(data => data)
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