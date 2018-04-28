using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess.Support
{
    public abstract class AbstractNonCommittingUnitOfWork : AbstractSingleCommitAsyncUnitOfWork
    {
        protected override Task CommitAsyncOnce(CancellationToken cancellationToken)
        {
            // No-op
            return Task.CompletedTask;
        }
    }

    public class ActionDisposeUnitOfWork : AbstractNonCommittingUnitOfWork
    {
        public ActionDisposeUnitOfWork(Action<bool> onDispose)
        {
            OnDispose = onDispose;
        }

        public Action<bool> OnDispose { get; }

        protected override void Dispose(bool disposing)
        {
            OnDispose?.Invoke(disposing);
        }
    }
}