using Microsoft.AspNetCore.Hosting;
using SimpleLiveData.App.Hosting;

namespace SimpleLiveData
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