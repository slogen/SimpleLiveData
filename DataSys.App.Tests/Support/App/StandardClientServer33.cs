using System.Threading.Tasks;

namespace DataSys.App.Tests.Test
{
    public class StandardClientServer33: StandardClientServerStateContext
    {
        protected override async Task Prepare()
            => await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);

    }
}