using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Datasys.App.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        private static IWebHostBuilder BuildWebHost(string[] args)
        {
            System.Console.WriteLine("B");
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseEnvironment(EnvironmentName.Development);
        }

        public class Startup : DataSys.App.Hosting.Startup
        {
        }
    }
}