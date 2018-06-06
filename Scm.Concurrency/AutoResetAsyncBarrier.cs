namespace Scm.Concurrency
{
    public class AutoResetAsyncBarrier: AbstractAsyncBarrier
    {
        public AutoResetAsyncBarrier(int waitCount) : base(waitCount)
        {
        }

        protected override void BarrierReached() 
        {
            // New callers will be queued untill the barrier is reached again
            Reset();
        }
        protected override void BarrierCancelled()
        {
            // Just the same as when reached. New callers will be queued untill the barrier is reached again
            BarrierReached();
        }
    }

}