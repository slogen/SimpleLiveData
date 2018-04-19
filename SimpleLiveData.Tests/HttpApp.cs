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
    public class HttpApp : AbstractHttpConfigurationFixture, IClassFixture<HttpApp>
    {
        public IScheduler Scheduler = new FastScheduler(1);
        public TestSource TestSource = new TestSource();

        public virtual IDataUnitOfWork DataUnitOfWork(IServiceProvider sp)
        {
            return TestSource;
        }

        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
        {
            var provider = new SchedulerProvider();
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