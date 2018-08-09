using System;
using System.Diagnostics.CodeAnalysis;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using DataSys.App.DataStorage;
using DataSys.App.Presentation.SignalR;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Scm.Presentation.Mvc.Parquet;

namespace DataSys.App.Hosting
{
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(cfg => { cfg.UseInMemoryDatabase("appdb"); });
            services.Add(ServiceDescriptor.Singleton(new AppSubjectContext()));
            services.Add(ServiceDescriptor.Scoped<IAppUnitOfWork>(
                sp => new AppUnitOfWork(sp.GetService<AppDbContext>(), sp.GetService<AppSubjectContext>())));
            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(DefaultParquetOutputFormatter.Default);
            });
            services.AddSignalR()
                .AddJsonProtocol(cfg =>
                    cfg.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddODataQueryFilter().AddOData();
        }

        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral", Justification = "Clarity")]
        [SuppressMessage("ReSharper", "ArgumentsStyleOther", Justification = "Clarity")]
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            // Any connection or hub wire up and configuration should go here
            app
                .UseSignalR(routes => routes.MapHub<LiveHub>(LiveHub.Route))
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