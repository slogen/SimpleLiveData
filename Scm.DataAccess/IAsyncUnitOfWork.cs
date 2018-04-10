using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    // Represents one unified block of stuff that should either go in or out
    public interface IAsyncUnitOfWork : IDisposable
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}