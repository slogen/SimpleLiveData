﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
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
            services.AddODataQueryFilter();
            services.AddOData();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapODataServiceRoute(
                    routeName: "odataserviceroute",
                    routePrefix: "odata",
                    model: BuildEdmModel(app.ApplicationServices));
            });
            
            // Any connection or hub wire up and configuration should go here
            app.UseSignalR(routes => { });
        }

        public IEdmModel BuildEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<Installation>(nameof(Installation));
            return builder.GetEdmModel();
        }
    }
}