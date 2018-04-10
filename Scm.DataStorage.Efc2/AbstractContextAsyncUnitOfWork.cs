using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scm.DataAccess;

namespace Scm.DataStorage.Efc2
{
    public abstract class AbstractContextAsyncUnitOfWork<TDbContext> : AbstractSingleCommitAsyncUnitOfWork
        where TDbContext : DbContext
    {
        protected abstract TDbContext Context { get; }

        protected override async Task CommitAsyncOnce(CancellationToken cancellationToken)
            => await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Context.Dispose();
        }
    }
}