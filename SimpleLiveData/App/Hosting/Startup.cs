using System.Web.Http;
using Owin;

namespace SimpleLiveData.App.Hosting
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
            // Any connection or hub wire up and configuration should go here
            appBuilder.MapSignalR();
        }
    }
}