using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DataSys.App.Presentation.Security;
using DataSys.App.Tests.Support;
using FluentAssertions;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class PositiveSecurityTests : SecurityTestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public PositiveSecurityTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        protected async Task<List<IdentityController.ClaimHeld>> Query(string path)
        {
            var server = Server;
            return await Client.GetJsonAsync<List<IdentityController.ClaimHeld>>(
                server.BaseAddress.Slash(IdentityController.RoutePrefix).Slash(path),
                JsonSerializer,
                CancellationToken
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task ClaimsShouldBeEmptyWhenNoAuthorizeRequired()
        {
            var got = await Query(IdentityController.NoAuthorizeRoute).ConfigureAwait(false);
            got.Should().BeEmpty();
        }

        [Fact]
        [Trait("Bug", "Not Implemented")]
        public async Task ClaimsShouldContainRoleAWhenAuthorizeRequired()
        {
            var got = await Query(IdentityController.RoleAAuthorizeRoute).ConfigureAwait(false);
            // TODO: Rewrite to the right kind of assertion
            got.Should().Contain(r => r.ValueType == "role" && r.Value == "roleA");
        }

        [Fact]
        [Trait("Bug", "Not Implemented")]
        public async Task ClaimsShouldContainUserWhenAuthorizeRequired()
        {
            var got = await Query(IdentityController.AuthorizeRoute).ConfigureAwait(false);
            got.Should().NotBeEmpty();
        }
    }
}