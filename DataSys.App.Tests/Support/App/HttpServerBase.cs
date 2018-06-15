using DataSys.App.Tests.Support.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace DataSys.App.Tests.Support.App
{
    public abstract class HttpServerBase<TStartup> : HttpServerBase
        where TStartup : class
    {
        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
            => base.ConfigureBuilder(builder).UseStartup<TStartup>();
    }
}