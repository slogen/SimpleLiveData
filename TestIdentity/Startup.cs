using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSys.App.Tests.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TestIdentity
{
    public class Startup: SecurityTestSourceBasedTests.TestStartup {
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }
}
