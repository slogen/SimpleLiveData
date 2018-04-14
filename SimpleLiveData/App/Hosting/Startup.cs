using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleLiveData.App.DataModel;
using SimpleLiveData.App.Presentation.SignalR;

namespace SimpleLiveData.App.Hosting
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddMvc();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            // Any connection or hub wire up and configuration should go here
            app.UseSignalR(routes => { });
        }
    }
}