using System.Threading;

namespace Scm.Concurrency
{
    public class CancellationSourceScope : ICancellationScope
    {
        public CancellationSourceScope(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        public CancellationTokenSource CancellationTokenSource { get; }

        public void Dispose()
        {
            CancellationTokenSource.Dispose();
        }

        public CancellationToken Token => CancellationTokenSource.Token;
    }
}