using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    public interface ITwoPhaseUnitOfWork: IDisposable
    {
        Task<ITransactionReady> Prepare(CancellationToken cancellationToken);
    }
}
