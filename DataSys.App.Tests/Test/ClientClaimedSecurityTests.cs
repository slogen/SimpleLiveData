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
    [Collection(nameof(StandardClientServerStateSecurityContextWithAuth))]
    public class ClientClaimedSecurityTests : InClientServerStateContextTest<StandardClientServerStateSecurityContextWithAuth>
    {
        [Fact]
        public async Task ClaimsShouldContainClientIdWhenAuthorizeRequired()
        {
            var got = await Context.Query(IdentityController.AuthorizeRoute).ConfigureAwait(false);

            got.Identities.Single().Claims
                .Should().Contain(c => c.Value == Context.ExpectClientId && c.Type == "client_id");
        }
        [Fact]
        public async Task ClaimsShouldContainClientIdNoWhenAuthorizeRequired()
        {
            var got = await Context.Query(IdentityController.NoAuthorizeRoute).ConfigureAwait(false);

            got.Identities.Single().Claims
                .Should().Contain(c => c.Value == Context.ExpectClientId && c.Type == "client_id");
        }

        [Fact]
        public void RequestsForGroupNotHeldShouldCause403()
        {
            IdentityController.RoleAAuthorizeRoute
                .Awaiting(async route => await Context.Query(route).ConfigureAwait(false))
                .Should().Throw<HttpResponseException>()
                .Which.Response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        public ClientClaimedSecurityTests(StandardClientServerStateSecurityContextWithAuth context) : base(context)
        {
        }
    }
}