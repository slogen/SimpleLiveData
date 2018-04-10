using System.Threading;

namespace Scm.Concurrency
{
    public class CancellationTokenScope : ICancellationScope
    {
        public CancellationTokenScope(CancellationToken cancellationToken)
        {
            Token = cancellationToken;
        }

        public CancellationToken Token { get; }

        public void Dispose()
        {
            /* no-op */
        }
    }
}