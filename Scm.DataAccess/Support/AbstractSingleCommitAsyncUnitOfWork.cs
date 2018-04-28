using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess.Support
{
    /// <summary>
    /// Enforces that a UnitOfWork is (successfully) committed only once
    /// </summary>
    public abstract class AbstractSingleCommitAsyncUnitOfWork : IAsyncUnitOfWork
    {
        private TaskCompletionSource<int> _committing;
        private int _disposeCount;

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (_disposeCount > 0)
                throw new ObjectDisposedException($"{GetType()} disposed");
            var thisCommit = new TaskCompletionSource<int>();
            var wasThere = Interlocked.CompareExchange(ref _committing, thisCommit, null);
            if (!(wasThere is null))
            {
                if (wasThere.Task.IsCompleted)
                    throw new InvalidOperationException("A *unit* can only be committed once");
                throw new InvalidOperationException("A commit is already on progress");
            }

            try
            {
                await CommitAsyncOnce(cancellationToken).ConfigureAwait(false);
                thisCommit.TrySetResult(0);
            }
            finally
            {
                if (thisCommit.Task.Status != TaskStatus.RanToCompletion)
                    if (Interlocked.CompareExchange(ref _committing, null, thisCommit) != thisCommit)
                        throw new InvalidOperationException("Multiple concurrent commits detected. Should not happen!");
            }
        }

        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposeCount) == 1)
                Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract Task CommitAsyncOnce(CancellationToken cancellationToken);

        protected abstract void Dispose(bool disposing);

        ~AbstractSingleCommitAsyncUnitOfWork()
        {
            Dispose(false);
        }
    }
}