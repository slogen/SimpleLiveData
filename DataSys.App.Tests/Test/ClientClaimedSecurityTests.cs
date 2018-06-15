using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataSys.App.Presentation.Security;
using DataSys.App.Tests.Support.App;
using FluentAssertions;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class ClientClaimedSecurityTests : IdentityControllerSecurityTests
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public ClientClaimedSecurityTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        [Fact]
        public async Task ClaimsShouldContainClientIdWhenAuthorizeRequired()
        {
            var got = await Query(IdentityController.AuthorizeRoute).ConfigureAwait(false);

            got.Identities.Single().Claims
                .Should().Contain(c => c.Value == ExpectClientId && c.Type == "client_id");
        }

        [Fact]
        public void RequestsForGroupNotHeldShouldCause403()
        {
            IdentityController.RoleAAuthorizeRoute
                .Awaiting(async route => await Query(route).ConfigureAwait(false))
                .Should().Throw<HttpResponseException>()
                .Which.Response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}