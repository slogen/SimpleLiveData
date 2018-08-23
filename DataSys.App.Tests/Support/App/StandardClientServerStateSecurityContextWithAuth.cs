using DataSys.App.Tests.Test;
using Xunit;

namespace DataSys.App.Tests.Support.App
{
    [CollectionDefinition(nameof(StandardClientServerStateSecurityContextWithAuth))]
    public class StandardClientServerStateSecurityContextWithAuth :
        StandardClientServerStateSecurityContext,
        ICollectionFixture<StandardClientServerStateSecurityContextWithAuth>
    {

    }
}