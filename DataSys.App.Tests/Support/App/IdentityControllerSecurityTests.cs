using System.Linq;
using System.Threading.Tasks;
using DataSys.App.DataAccess;
using DataSys.App.Presentation.Security;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Support.App
{
    public abstract class IdentityControllerSecurityTests : SecurityTestSourceBasedTests,
        IClassFixture<TestAppUnitOfWorkFactory>
    {
        protected IdentityControllerSecurityTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(
            appUnitOfWorkFactory)
        {
        }

        protected string ExpectIssuer => IdServer.BaseAddress.ToString();
        protected string ExpectClientName => Identity4ServerConfiguration.Id4Clients.Single().ClientName;
        protected string ExpectClientId => Identity4ServerConfiguration.Id4Clients.Single().ClientId;

        protected async Task<IdentityController.HeldClaimsPrincipal> Query(string path)
        {
            var server = Server;
            return await Client.GetJsonAsync<IdentityController.HeldClaimsPrincipal>(
                server.BaseAddress.Slash(IdentityController.RoutePrefix).Slash(path),
                JsonSerializer,
                CancellationToken
            ).ConfigureAwait(false);
        }
    }
}