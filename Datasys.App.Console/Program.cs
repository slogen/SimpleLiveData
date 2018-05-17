using DataSys.App.Tests.Test;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Datasys.App.Console
{
    public class Startup : SecurityTestSourceBasedTests.TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            base.Configure(app, env);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("B1");
            //WebHost.CreateDefaultBuilder(args)
            new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
