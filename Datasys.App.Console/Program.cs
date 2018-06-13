using System.Diagnostics.CodeAnalysis;
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
            BuildA(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Experimentally exchanging")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Passed for uniformity")]
        private static IWebHostBuilder BuildA(string[] args)
        {
            System.Console.WriteLine("A");
            return new WebHostBuilder()
                .UseKestrel()
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseEnvironment(EnvironmentName.Development);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Experiments exchange A and B")]
        private static IWebHostBuilder BuildB(string[] args)
        {
            System.Console.WriteLine("B");
            return WebHost.CreateDefaultBuilder(args);
        }

        public class Startup : SecurityTestSourceBasedTests.Startup
        {
            public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
                base.Configure(app, env);
            }
        }
    }
}