using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.Presentation.Security;
using DataSys.App.Tests.Support.App;
using FluentAssertions;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class NoClaimSecurityTests : IdentityControllerSecurityTests
    {
        // ReSharper disable once SuggestBaseTypeForParameter -- concrete type required by xunit test injection
        public NoClaimSecurityTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        protected override async Task ConfigureClientAuthentication(HttpClient client,
            CancellationToken cancellationToken)
        {
            // Don't authenticate
            await Task.Yield();
        }

        [Fact]
        public async Task RequestsWithoutAuthorizationShouldPassWithUnauthenticatedClient()
        {
            var got = await Query(IdentityController.NoAuthorizeRoute).ConfigureAwait(false);
            got.Should().NotBeNull();
        }
    }
}