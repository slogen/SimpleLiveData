using System;
using DataSys.App.DataAccess;
using DataSys.App.DataStorage;
using DataSys.App.Hosting;
using DataSys.App.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    public class TestSourceBasedTests : DataAppTests<Startup>
    {
        private TestSource _testSource;
        protected TestSource TestSource => _testSource ?? (_testSource = MakeTestSource());
        protected JsonSerializer JsonSerializer { get; } = new JsonSerializer();
        protected virtual TestSource MakeTestSource() => new TestSource();

        public IAppUnitOfWork DataUnitOfWork(IServiceProvider sp)
        {
            return sp.GetService<AppUnitOfWork>();
        }

        protected override void ConfigureTestServices(IServiceCollection svcs)
        {
            base.ConfigureTestServices(svcs);
            svcs.AddDbContextPool<AppDbContext>(cfg => { cfg.UseInMemoryDatabase("appdb"); });
        }
    }
}