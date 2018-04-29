using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Scm.Presentation.OData;
using Scm.Rx;

namespace DataSys.App.Tests.Support
{
    /// <summary>
    /// Base class to setup tests for Data Applications.
    /// 
    /// Provides configuration of sched
    /// </summary>
    public abstract class DataAppTests<TStartup> : AbstractHttpConfigurationFixture
        where TStartup : class
    {
        private IScheduler _scheduler;
        protected CancellationToken CancellationToken => CancellationToken.None;
        protected virtual IScheduler Scheduler => _scheduler ?? (_scheduler = MakeScheduler());
        protected virtual IScheduler MakeScheduler() => new FastScheduler(1000);

        protected virtual async Task OnRequest(HttpContext context, RequestDelegate next)
        {
            await next(context).ConfigureAwait(false);
        }

        protected virtual void ConfigureTestServices(IServiceCollection svcs)
        {
            svcs.Add(ServiceDescriptor.Singleton(typeof(IScheduler), Scheduler));
            svcs.Add(ServiceDescriptor.Singleton(typeof(IODataOptions),
                new ODataOptions
                {
                    SchedulerProvider = SchedulerProvider.Singular(Scheduler)
                }));
        }

        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
        {
            return builder
                    // Easier breakpoints on requests to debug routing
                    .Configure(app => app.Use(next => context => OnRequest(context, next)))
                    // Configure stuff used in tests
                    .ConfigureTestServices(ConfigureTestServices)
                    // Dispatch to "real" startup
                    .UseStartup<TStartup>()
                ;
        }
    }
}