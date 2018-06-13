using DataSys.App.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace DataSys.App.Tests.Test
{
    public class SecurityTestSourceBasedTests : TestSourceBasedTests<SecurityTestSourceBasedTests.Startup>
    {
        public SecurityTestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }
        public class Startup : TestSourceBasedTests.Startup
        {
            public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseAuthentication();
                base.Configure(app, env);
            }
        }

    }
}