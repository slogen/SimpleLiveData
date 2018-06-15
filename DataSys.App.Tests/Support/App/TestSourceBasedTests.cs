using DataSys.App.DataAccess;
using DataSys.App.Tests.Support;
using DataSys.App.Tests.Support.App;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DataSys.App.Tests.Test
{
    public class TestSourceBasedTests : TestSourceBasedTests<TestSourceBasedTests.Startup>
    {
        public TestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }
        public class Startup : Hosting.Startup
        {

        }
    }


    public class TestSourceBasedTests<TStartup> : DataAppTests<TStartup>
        where TStartup : class
    {
        private TestSource _testSource;

        private static Logger Logger { get; } = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //.MinimumLevel.Override("System", LogEventLevel.Warning)
            //.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: ConsoleTheme.None)
            .CreateLogger();

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