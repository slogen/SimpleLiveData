using DataSys.App.Tests.Test;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Datasys.App.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildB(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        static IWebHostBuilder BuildA(string[] args)
        {
            System.Console.WriteLine("A");
            return new WebHostBuilder()
                .UseKestrel()
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseEnvironment(EnvironmentName.Development);
        }

        static IWebHostBuilder BuildB(string[] args)
        {
            System.Console.WriteLine("B");
            return WebHost.CreateDefaultBuilder(args);
        }

        public class Startup : SecurityTestSourceBasedTests.TestStartup
        {
            public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
                base.Configure(app, env);
            }
        }
    }
}