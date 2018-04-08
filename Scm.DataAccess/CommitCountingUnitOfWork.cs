using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    public abstract class CommitCountingUnitOfWork : IUnitOfWork
    {
        private int _commitCount;
        private int _disposeCount;
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _commitCount) != 1)
                throw new InvalidOperationException("A *unit* can only be committed once");
            await CommitAsyncOnce(cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task CommitAsyncOnce(CancellationToken cancellationToken);

        protected abstract void Dispose(bool disposing);
        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposeCount) == 1)
                Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CommitCountingUnitOfWork() { Dispose(false); }
    }
}
