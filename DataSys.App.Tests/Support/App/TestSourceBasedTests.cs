using DataSys.App.DataAccess;
using DataSys.App.Tests.Support.App.Source;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace DataSys.App.Tests.Support.App
{
    public class TestSourceBasedTests : TestSourceBasedTests<TestSourceBasedTests.Startup>
    {
        public TestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }
        public class Startup : DataSys.App.Hosting.Startup
        {

        }
    }


    public class TestSourceBasedTests<TStartup> : DataAppTests<TStartup>
        where TStartup : class
    {
        private TestSource _testSource;

        public TestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory)
        {
            AppUnitOfWorkFactory = appUnitOfWorkFactory;
        }

        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
        {
            return base.ConfigureBuilder(builder).UseSerilog();
        }

        public IAppUnitOfWorkFactory AppUnitOfWorkFactory { get; }
        protected TestSource TestSource => _testSource ?? (_testSource = MakeTestSource());
        protected JsonSerializer JsonSerializer { get; } = new JsonSerializer();
        protected virtual TestSource MakeTestSource() => new TestSource(AppUnitOfWorkFactory);

        protected override void ConfigureTestServices(IServiceCollection svcs)
        {
            svcs.Replace(ServiceDescriptor.Scoped(sp => AppUnitOfWorkFactory.UnitOfWork()));
            base.ConfigureTestServices(svcs);
        }
    }
}