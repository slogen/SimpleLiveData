using Scm.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    public static class TwoPhaseAsUnitOfWorkExtensions
    {
        public static async Task SaveChangesAsync(this ITwoPhaseUnitOfWork twoPhaseUnitOfWork, CancellationToken cancellationToken)
        {
            await twoPhaseUnitOfWork.ToUnitOfWork().SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        public static IAsyncUnitOfWork ToUnitOfWork(this ITwoPhaseUnitOfWork twoPhaseUnitOfWork) => new TwoPhaseAsUnitOfWork(twoPhaseUnitOfWork);
        private class TwoPhaseAsUnitOfWork: IAsyncUnitOfWork
        {
            private ITwoPhaseUnitOfWork Parent { get; }
            public TwoPhaseAsUnitOfWork(ITwoPhaseUnitOfWork twoPhaseUnitOfWork) {
                Parent = twoPhaseUnitOfWork ?? throw new ArgumentNullException(nameof(twoPhaseUnitOfWork));
            }
            public async Task SaveChangesAsync(CancellationToken cancellationToken)
            {
                var ready = await Parent.Prepare(cancellationToken).ConfigureAwait(false);
                await ready.Commit(cancellationToken).ConfigureAwait(false);
            }

            public void Dispose()
            {
                Parent.Dispose();
            }
        }
        private class CombineTwoPhases: ITwoPhaseUnitOfWork {
            private IEnumerable<ITwoPhaseUnitOfWork> UnitsOfWork { get; }
            public CombineTwoPhases(IEnumerable<ITwoPhaseUnitOfWork> unitsOfWork)
            {
                UnitsOfWork = unitsOfWork ?? throw new ArgumentNullException(nameof(unitsOfWork));
            }

            private class CombineTransactionReadies: ITransactionReady
            {
                private IEnumerable<ITransactionReady> TransactionReadies { get; }
                public CombineTransactionReadies(IEnumerable<ITransactionReady> transactionReadies)
                {
                    TransactionReadies = transactionReadies ?? throw new ArgumentNullException(nameof(transactionReadies));  
                }
                public async Task Commit(CancellationToken cancellationToken)
                {
                    // These commits *should* not fail, they have been prepared
                    await Task.WhenAll(TransactionReadies.Select(t => t.Commit(cancellationToken))).ConfigureAwait(false);
                }

                public async Task Rollback(CancellationToken cancellationToken)
                {
                    await Task.WhenAll(TransactionReadies.Select(t => t.Rollback(cancellationToken))).ConfigureAwait(false);
                }
            }

            public async Task<ITransactionReady> Prepare(CancellationToken cancellationToken)
             => new CombineTransactionReadies(await Task.WhenAll(UnitsOfWork.Select(uow => uow.Prepare(cancellationToken))));

            public void Dispose() => UnitsOfWork.Reverse().DisposeAll();
        }
    }
}
