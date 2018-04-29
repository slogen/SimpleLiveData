using System;
using System.Diagnostics.CodeAnalysis;
using DataSys.App.DataModel;
using DataSys.App.Presentation.SignalR;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Newtonsoft.Json;

namespace DataSys.App.Hosting
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR()
                .AddJsonProtocol(cfg =>
                    cfg.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddODataQueryFilter();
            services.AddOData();
            services.AddMvc();
        }

        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral", Justification = "Clarity")]
        [SuppressMessage("ReSharper", "ArgumentsStyleOther", Justification = "Clarity")]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            // Any connection or hub wire up and configuration should go here
            app
                .UseSignalR(routes => routes.MapHub<LiveHub>("/signalr/livedata"))
                .UseMvc(routes =>
                {
                    routes.MapODataServiceRoute(
                        routeName: "odataserviceroute",
                        routePrefix: "odata",
                        model: BuildEdmModel(app.ApplicationServices));
                });
        }

        public IEdmModel BuildEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<Installation>(nameof(Installation));
            builder.EntitySet<Signal>(nameof(Signal));
            return builder.GetEdmModel();
        }
    }
}