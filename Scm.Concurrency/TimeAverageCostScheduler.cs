using System;

namespace Scm.Concurrency
{
    public class TimeAverageCostScheduler: AbstractTimeAverageCostScheduler
    {
        public TimeAverageCostScheduler(TimeSpan averagePeriod, double desiredAverage, IClock clock)
        {
            AveragePeriod = averagePeriod;
            DesiredAverage = desiredAverage;
            Clock = clock;
        }

        public override TimeSpan AveragePeriod { get; }

        public override double DesiredAverage { get; }
        public override IClock Clock { get; }
    }
}