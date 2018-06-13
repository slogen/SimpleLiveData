using System.Reactive.Concurrency;
using DataSys.App.Tests.Support.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Scm.Presentation.OData;
using Scm.Rx;

namespace DataSys.App.Tests.Support
{
    public abstract class HttpServerBase<TStartup>: HttpServerBase
        where TStartup: class
    {
        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
            => base.ConfigureBuilder(builder).UseStartup<TStartup>();
    }

    /// <summary>
    /// Base class to setup tests for Data Applications.
    /// 
    /// Provides configuration of sched
    /// </summary>
    public abstract class DataAppTests<TStartup>: HttpServerBaseWithId<TStartup>
        where TStartup: class
    {
        private IScheduler _scheduler;
        protected virtual IScheduler Scheduler => _scheduler ?? (_scheduler = MakeScheduler());
        protected virtual IScheduler MakeScheduler() => new FastScheduler(1000);

        protected override IIdentity4ServerConfiguration Identity4ServerConfiguration => TestIdentityServerConfiguration.Default;

        protected override void ConfigureTestServices(IServiceCollection svcs)
        {
            base.ConfigureTestServices(svcs);
            RegisterScheduler(svcs);
            RegisterOdata(svcs);
        }
        protected virtual void RegisterScheduler(IServiceCollection svcs)
        {
            svcs.Add(ServiceDescriptor.Singleton(typeof(IScheduler), Scheduler));
        }
        protected virtual void RegisterOdata(IServiceCollection svcs)
        {
            svcs.Add(ServiceDescriptor.Singleton(typeof(IODataOptions),
                new ODataOptions
                {
                    SchedulerProvider = SchedulerProvider.Singular(Scheduler)
                }));
        }

    }
}