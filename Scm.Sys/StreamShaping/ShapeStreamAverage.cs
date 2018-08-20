using System;
using Scm.Concurrency;

namespace Scm.Sys.StreamShaping
{
    public class ShapeStreamAverage: ShapeStreamSums
    {
        public static TimeSpan DefaultAverageSpan = TimeSpan.FromSeconds(3);
        public ShapeStreamAverage(IStreamCost costs, double sharedBudget, double? writeBudget = null, double? seekBudget = null, TimeSpan? averageSpan = null, IClock clock = null)
            : base(costs, StreamExtensions.AveragingSchedulersPerSecond(sharedBudget, writeBudget, seekBudget, averageSpan, clock))
        {
        }
    }
}