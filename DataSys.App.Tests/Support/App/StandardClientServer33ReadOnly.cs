using DataSys.App.Tests.Test;
using Xunit;

namespace DataSys.App.Tests.Support.App
{
    /// <inheritdoc cref="StandardClientServerStateContext" />
    /// <summary>
    /// Specific server setup with 3 installations and 3 signals.
    /// No savechanges allowed, as it is shared (via collection) between multiple tests that read the DB-state
    /// </summary>
    [CollectionDefinition(nameof(StandardClientServer33ReadOnly))]
    public class StandardClientServer33ReadOnly : StandardClientServer33, ICollectionFixture<StandardClientServer33ReadOnly>
    {
    }
}