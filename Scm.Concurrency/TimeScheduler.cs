namespace Scm.Concurrency
{
    public class TimeScheduler : AbstractTimeScheduler
    {
        public TimeScheduler(ITimeBudgetScheduler timeBudgetScheduler, IClock clock) {
            TimeBudgetScheduler = timeBudgetScheduler;
            Clock = clock ?? SystemClockUtc.Default;
        }
        public override IClock Clock { get; }

        public override ITimeBudgetScheduler TimeBudgetScheduler { get; }
    }
}