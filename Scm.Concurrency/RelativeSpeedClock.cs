using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    /// <summary>
    /// Clock that is relative to another clock
    /// 
    /// Useful for tests
    /// </summary>
    public class RelativeSpeedClock: IClock
    {
        public IClock Parent { get; }
        public double Speedup { get; }
        public DateTime Epoch { get; }

        public RelativeSpeedClock(IClock parent, double speedup, DateTime? epoch = null)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (double.IsInfinity(speedup) || double.IsNaN(speedup))
                throw new ArgumentException($"Unsupported speedup value: {speedup}", nameof(speedup));
            Speedup = speedup;
            Epoch = epoch ?? parent.Now();
            Parent = parent;
        }

        public DateTime Now()
        {
            var n = Parent.Now();
            return n + FromParent(n - Epoch);
        }
        protected TimeSpan FromParent(TimeSpan span) => TimeSpan.FromSeconds(span.TotalSeconds * Speedup);
        protected TimeSpan ToParent(TimeSpan span) => TimeSpan.FromSeconds(span.TotalSeconds / Speedup);

        public async Task Delay(TimeSpan span, CancellationToken cancellationToken)
            => await Parent.Delay(ToParent(span), cancellationToken).ConfigureAwait(false);

        public void Sleep(TimeSpan span) => Parent.Sleep(ToParent(span));
    }
}
