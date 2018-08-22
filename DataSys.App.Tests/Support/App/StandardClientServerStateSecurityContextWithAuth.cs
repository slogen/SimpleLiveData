using Xunit;

namespace DataSys.App.Tests.Test
{
    /// <inheritdoc />
    [CollectionDefinition(nameof(StandardClientServerStateSecurityContextWithAuth))]
    public class StandardClientServerStateSecurityContextWithAuth :
        StandardClientServerStateSecurityContext,
        ICollectionFixture<StandardClientServerStateSecurityContextWithAuth>
    {

    }
}