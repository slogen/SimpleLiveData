using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Scm.Web;
using SimpleLiveData.App.DataModel;
using SimpleLiveData.App.Presentation.SignalR;

namespace SimpleLiveData.App.Hosting
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR()
                .AddJsonProtocol(cfg =>
                    cfg.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddMvc();
            services.AddODataQueryFilter();
            services.AddOData();
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