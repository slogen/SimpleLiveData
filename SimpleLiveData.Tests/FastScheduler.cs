using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace SimpleLiveData.Tests
{
    public class FastScheduler : VirtualTimeScheduler<DateTime, double>
    {
        public FastScheduler(double speedup) : base(DateTime.UtcNow, Comparer<DateTime>.Default)
        {
            Speedup = speedup;
        }

        public double Speedup { get; }

        protected override DateTime Add(DateTime absolute, double relative)
            => absolute.Add(TimeSpan.FromSeconds(relative));

        protected override DateTimeOffset ToDateTimeOffset(DateTime absolute)
            => new DateTimeOffset(absolute);

        protected override double ToRelative(TimeSpan timeSpan)
            => timeSpan.TotalSeconds / Speedup;
    }
}