using System.Linq;
using System.Threading.Tasks;
using DataSys.App.Presentation.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Scm.Web;

namespace DataSys.App.Tests.Test
{

    public class StandardClientServerStateSecurityContext : 
        StandardClientServerStateContext<StandardClientServerStateSecurityContext.Startup>
    {
        public string ExpectIssuer => IdServer.BaseAddress.ToString();
        public string ExpectClientName => Identity4ServerConfiguration.Id4Clients.Single().ClientName;
        public string ExpectClientId => Identity4ServerConfiguration.Id4Clients.Single().ClientId;

        public async Task<IdentityController.HeldClaimsPrincipal> Query(string path)
        {
            var server = Server;
            return await Client.GetJsonAsync<IdentityController.HeldClaimsPrincipal>(
                server.BaseAddress.Slash(IdentityController.RoutePrefix).Slash(path),
                JsonSerializer,
                CancellationToken
            ).ConfigureAwait(false);
        }

        public class Startup : StandardClientServerStateContext.Startup
        {
            public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseAuthentication();
                base.Configure(app, env);
            }
        }

        protected override Task Prepare() => Task.CompletedTask;
    }
}