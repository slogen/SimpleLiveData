#if _LATER
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Scm.DataAccess
{
    public interface ITwoPhaseUnitOfWork: IDisposable, IPromotableSinglePhaseNotification
    {
        Task<ITransactionReady> Prepare(CancellationToken cancellationToken);
    }
}
#endif