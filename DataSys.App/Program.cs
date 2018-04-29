using DataSys.App.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace DataSys.App
{
    internal class Program
    {
        private static void Main()
        {
            new WebHostBuilder()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}