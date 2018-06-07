using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public abstract class AbstractAsyncBarrier
    {
        private int _remain;
        private int _waitCount;
        private TaskCompletionSource<int> Reached;

        protected AbstractAsyncBarrier(int waitCount)
        {
            if (waitCount <= 0)
                throw new ArgumentException($"Cannot wait for {waitCount}", nameof(waitCount));
            WaitCount = waitCount;
            _remain = waitCount;
        }

        public int WaitCount
        {
            get { return _waitCount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Cannot wait for {value}", "value");
                lock (this)
                {
                    _waitCount = value;
                    Reset();
                }
            }
        }

        public int Remain
        {
            get { return _remain; }
            protected set
            {
                Contract.Assert(Monitor.IsEntered(this));
                _remain = value;
            }
        }

        protected abstract void BarrierReached();
        protected abstract void BarrierCancelled();

        public void Reset()
        {
            lock (this)
            {
                _remain = WaitCount;
                var source = Reached;
                if (source != null)
                    source.TrySetCanceled();
                Reached = null;
            }
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource<int> source;
            // Atomically calculate whether that caused is to complete
            lock (this)
            {
                source = Reached;
                if (--_remain <= 0)
                {
                    if (source != null)
                        source.TrySetResult(0);
                    BarrierReached();
                    return source?.Task ?? Task.CompletedTask;
                }

                if (source == null)
                    Reached = source = new TaskCompletionSource<int>();
            }

            // We now know that completion is waiting 
            if (cancellationToken.CanBeCanceled)
            {
                // If waiting is cancelled, pass that on to all other waiters
                var cancelReg = cancellationToken.Register(() =>
                {
                    source.TrySetCanceled();
                    lock (this)
                    {
                        BarrierCancelled();
                    }
                });
                // Make sure we unregister the cancellation registration
                source.Task.ContinueWith(_ => cancelReg.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
            }

            return source.Task;
        }
    }
}