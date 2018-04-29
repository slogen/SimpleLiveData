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
}