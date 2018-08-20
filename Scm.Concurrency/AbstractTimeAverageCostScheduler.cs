using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Scm.Concurrency
{
    public abstract class AbstractTimeAverageCostScheduler: ITimeBudgetScheduler
    {
        public object SyncRoot => this;
        public abstract TimeSpan AveragePeriod
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public abstract double DesiredAverage
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public abstract IClock Clock
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        protected struct Cost
        {
            public DateTime At;
            public int Amount;
        }
        protected LinkedList<Cost> Costs = new LinkedList<Cost>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan NextSchedule(int cost)
        {
            // Update internal state, so we can esimate when next operation should be allowed
            lock (SyncRoot)
            {
                var now = Clock.Now();
                var backTo = now - AveragePeriod;
                if (cost != 0)
                    Costs.AddLast(new Cost { At = now, Amount = cost });
                while (Costs.Count > 0 && Costs.First.Value.At < backTo)
                    Costs.RemoveFirst();

                var spentTicksInAveragePeriod = Costs.Aggregate(0L, (a,c) => a+c.Amount) / DesiredAverage * AveragePeriod.Ticks;
                var ticksLeft = AveragePeriod.Ticks - (long)Math.Ceiling(spentTicksInAveragePeriod);
                var scheduleNext = -TimeSpan.FromTicks(ticksLeft);
                return scheduleNext;
            }
        }
    }
}