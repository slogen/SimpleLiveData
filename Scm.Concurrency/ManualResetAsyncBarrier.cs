namespace Scm.Concurrency
{
    public class ManualResetAsyncBarrier: AbstractAsyncBarrier
    {
        public ManualResetAsyncBarrier(int waitCount) : base(waitCount)
        {
        }

        protected override void BarrierReached()
        {
            // Callers will get current status of Reached until Reset()
        }
        protected override void BarrierCancelled()
        {
            // Just the same as reached, meaning Callers will get Cancelled until Reset()
        }
    }

}