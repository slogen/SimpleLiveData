#if _LATER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Scm.Concurrency;

namespace Scm.DataAccess
{
    public class TwoPhaseUnitOfWork : AbstractTwoPhaseUnitOfWork
    {
        public TwoPhaseUnitOfWork(params ITwoPhaseUnitOfWork[] participants) : this((ICollection<ITwoPhaseUnitOfWork>)participants) { }
        public TwoPhaseUnitOfWork(IEnumerable<ITwoPhaseUnitOfWork> participants) : this(participants.ToList()) { }
        public TwoPhaseUnitOfWork(ICollection<ITwoPhaseUnitOfWork> participants)
        {
            Participants = participants;
        }
        public override ICollection<ITwoPhaseUnitOfWork> Participants { get; }

        protected override Task CommitEachAsync(IEnumerable<ITransactionReady> ready,
            CancellationToken cancellationToken)
            => Task.WhenAll(ready.Select(rdy => rdy.Commit(cancellationToken)));
        protected override Task RollbackEachAsync(IEnumerable<ITransactionReady> ready,
            CancellationToken cancellationToken)
            => Task.WhenAll(ready.Select(rdy => rdy.Rollback(cancellationToken)));
    }
    public class TwoPhaseTransactionUnitOfWork : AbstractCancellationSupportingSingleCommitAsyncUnitOfWork, ITwoPhaseUnitOfWork
    {
        public CommittableTransaction Transaction { get; }

        protected ConcurrentDictionary<ITwoPhaseUnitOfWork, SinglePhaseEnlistmentNotification> Enlisted =
            new ConcurrentDictionary<ITwoPhaseUnitOfWork, SinglePhaseEnlistmentNotification>();

        public TwoPhaseTransactionUnitOfWork(CommittableTransaction transaction)
        {
            Transaction = transaction;
        }

        protected abstract class AbstractTwoPhaseEnlistmentNotification : IEnlistmentNotification
        {
            public Task CommitCompleted => _commitCompleted.Task;

            protected abstract ITwoPhaseUnitOfWork EnlistedUnitOfWork { get; }
            private readonly TaskCompletionSource<int> _commitCompleted = new TaskCompletionSource<int>();
            protected virtual void CompletedSucces() => _commitCompleted.TrySetResult(0);
            protected virtual void CompletedError(Exception ex) => _commitCompleted.TrySetException(ex);
            protected virtual void CompletedCancel() => _commitCompleted.TrySetCanceled();
            private ITransactionReady Ready { get; set; }

            protected abstract ICancellationScope CommitCancellationScope();

            protected abstract ICancellationScope PrepareCancellationScope();

            protected abstract ICancellationScope RollbackCancellationScope();

            protected void InTask(Func<Task> action)
            {
                action().GetAwaiter().GetResult();
            }
            public void Commit(Enlistment enlistment)
            {
                if (Ready == null)
                    throw new TransactionException("Not prepared");
                InTask(async () =>
                {
                    try
                    {
                        using (var cts = CommitCancellationScope())
                            await Ready.Commit(cts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        CompletedError(ex);
                        throw;
                    }
                    CompletedSucces();
                });
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                InTask(async () =>
                {
                    ITransactionReady rdy = null;
                    try
                    {
                        using (var cts = PrepareCancellationScope())
                            rdy = await EnlistedUnitOfWork.Prepare(cts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        CompletedError(ex);
                        preparingEnlistment.ForceRollback(ex);
                        throw;
                    }
                    Ready = rdy;
                });
                preparingEnlistment.Prepared();
            }

            public void Rollback(Enlistment enlistment)
            {
                if (Ready == null) // Just dispose without any action
                    EnlistedUnitOfWork.Dispose();
                else 
                    InTask(async () =>
                    {
                        try
                        {
                            using (var cts = RollbackCancellationScope())
                                await Ready.Rollback(cts.Token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            CompletedError(ex);
                            throw;
                        }
                    });
                enlistment.Done();
            }
        }

        protected abstract class AbstractSinglePhaseEnlistmentNotification: AbstractTwoPhaseEnlistmentNotification
        {
            public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
            {
                InTask(async () =>
                    {
                        try
                        {
                            using (var cts = CommitCancellationScope())
                                await (await EnlistedUnitOfWork.Prepare(cts.Token).ConfigureAwait(false)).Commit(cts.Token).ConfigureAwait(false);
                        }
                        catch
                        {
                            singlePhaseEnlistment.Aborted();
                            throw;
                        }
                        singlePhaseEnlistment.Committed();
                    });
            }
        }

        protected class SinglePhaseEnlistmentNotification : AbstractSinglePhaseEnlistmentNotification
        {
            private readonly TwoPhaseTransactionUnitOfWork _parent;
            private readonly IDisposable _cancellationRegistration;
            public CancellationToken ExternalCancellationToken { get; set; } = CancellationToken.None;
            protected override void CompletedSucces()
            {
                _cancellationRegistration.Dispose();
                base.CompletedSucces();
            }

            protected override void CompletedError(Exception ex)
            {
                _cancellationRegistration.Dispose();
                base.CompletedError(ex);
            }

            protected override void CompletedCancel()
            {
                _cancellationRegistration.Dispose();
                base.CompletedCancel();
            }


            public SinglePhaseEnlistmentNotification(TwoPhaseTransactionUnitOfWork parent, ITwoPhaseUnitOfWork enlistedEnlistedUnitOfWork)
            {
                _parent = parent;
                EnlistedUnitOfWork = enlistedEnlistedUnitOfWork;
                _cancellationRegistration = _parent.DisposeToken.Register(CompletedCancel);
            }
            protected override ITwoPhaseUnitOfWork EnlistedUnitOfWork { get; }
            protected ICancellationScope CancellationScope() => ExternalCancellationToken.Link(_parent.DisposeToken);

            protected override ICancellationScope CommitCancellationScope() => CancellationScope();
            protected override ICancellationScope PrepareCancellationScope() => CancellationScope();
            protected override ICancellationScope RollbackCancellationScope() => CancellationScope();
        }

        public void Enlist(ITwoPhaseUnitOfWork unitOfWork)
        {
            var n = new SinglePhaseEnlistmentNotification(this, unitOfWork);
            if (!Enlisted.TryAdd(unitOfWork, n))
                throw new InvalidOperationException($"UnitOfWork already enlisted: {unitOfWork}");
            Transaction.EnlistVolatile(n, EnlistmentOptions.EnlistDuringPrepareRequired);
        }
        protected override Task CommitAsyncOnce(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var n in Enlisted.Values)
                n.ExternalCancellationToken = cancellationToken;
            return Task.Factory.FromAsync(Transaction.BeginCommit, Transaction.EndCommit, default(object));
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    Transaction.Rollback();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public Task<ITransactionReady> Prepare(CancellationToken cancellationToken)
        {
        }
    }
}
#endif