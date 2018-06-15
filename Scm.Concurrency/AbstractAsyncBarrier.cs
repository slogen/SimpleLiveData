using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public abstract class AbstractAsyncBarrier
    {
        private TaskCompletionSource<int> _reached;
        private int _remain;
        private int _waitCount;

        protected AbstractAsyncBarrier(int waitCount)
        {
            if (waitCount <= 0)
                throw new ArgumentException($"Cannot wait for {waitCount}", nameof(waitCount));
            WaitCount = waitCount;
            _remain = waitCount;
        }

        public int WaitCount
        {
            get => _waitCount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Cannot wait for {value}", nameof(value));
                lock (this)
                {
                    _reached?.TrySetCanceled();
                    BarrierCancelled();
                    _waitCount = value;
                }
            }
        }

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter -- Better expressed as private variable and accessor
        public int Remain => _remain;

        protected abstract void BarrierReached();
        protected abstract void BarrierCancelled();

        public void Reset()
        {
            lock (this)
            {
                _remain = WaitCount;
                var source = _reached;
                source?.TrySetCanceled();
                _reached = null;
            }
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource<int> source;
            // Atomically calculate whether that caused is to complete
            lock (this)
            {
                source = _reached;
                if (--_remain <= 0)
                {
                    source?.TrySetResult(0);
                    BarrierReached();
                    return source?.Task ?? Task.CompletedTask;
                }

                if (source == null)
                    _reached = source = new TaskCompletionSource<int>();
            }

            // We now know that completion is waiting 
            // ReSharper disable once InvertIf -- Will not get more readable
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