using System;
using DataSys.App.DataAccess;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Test
{
    public class SecurityTestSourceBasedTests : TestSourceBasedTests<SecurityTestSourceBasedTests.Startup>
    {
        public SecurityTestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }
        public class Startup : TestSourceBasedTests.Startup
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                base.ConfigureServices(services);
            }

            public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseAuthentication();
                base.Configure(app, env);
            }
        }

    }
}