using DataSys.App.DataAccess;
using DataSys.App.Hosting;
using DataSys.App.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    public class TestSourceBasedTests : DataAppTests<Startup>
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

        protected override void ConfigureTestServices(IServiceCollection svcs)
        {
            svcs.Replace(ServiceDescriptor.Scoped(sp => AppUnitOfWorkFactory.UnitOfWork()));
            base.ConfigureTestServices(svcs);
        }
    }
}