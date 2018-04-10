using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    public interface ITransactionReady
    {
        Task Commit(CancellationToken cancellationToken);
        Task Rollback(CancellationToken cancellationToken);
    }
}