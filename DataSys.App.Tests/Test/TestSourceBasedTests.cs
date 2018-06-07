using DataSys.App.DataAccess;
using DataSys.App.Hosting;
using DataSys.App.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    public class TestSourceBasedTests : TestSourceBasedTests<TestSourceBasedTests.TestStartup>
    {
        public TestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        public class TestStartup : Startup
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

        public IAppUnitOfWorkFactory AppUnitOfWorkFactory { get; }
        protected TestSource TestSource => _testSource ?? (_testSource = MakeTestSource());
        protected JsonSerializer JsonSerializer { get; } = new JsonSerializer();
        protected virtual TestSource MakeTestSource() => new TestSource(AppUnitOfWorkFactory);

        protected override void PreConfigureTestServices(IServiceCollection svcs)
        {
            svcs.Replace(ServiceDescriptor.Scoped(sp => AppUnitOfWorkFactory.UnitOfWork()));
            base.PreConfigureTestServices(svcs);
        }
    }
}