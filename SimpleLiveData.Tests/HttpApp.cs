using System;
using System.Reactive.Concurrency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Scm.Presentation.OData;
using Scm.Rx;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.Hosting;
using Xunit;

namespace SimpleLiveData.Tests
{
    [CollectionDefinition("HttpApp")] // Dclare how to share a fully configured Http interface to App
    public class HttpApp : AbstractHttpConfigurationFixture, ICollectionFixture<HttpApp>
    {
        public IScheduler Scheduler = new FastScheduler(1);

        public virtual IDataUnitOfWork DataUnitOfWork(IServiceProvider sp)
        {
            return new TestSource();
        }
        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
        {
            var provider = new SchedulerProvider
            {
                // Default = Scheduler
            };
            return builder
                .UseStartup<Startup>()
                .ConfigureTestServices(svcs =>
                {
                    svcs.Add(ServiceDescriptor.Scoped(DataUnitOfWork));
                    svcs.Add(ServiceDescriptor.Singleton(typeof(IODataOptions),
                        new ODataOptions
                        {
                            SchedulerProvider = provider
                        }));
                });

        }
    }
}