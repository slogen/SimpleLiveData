using System;

namespace Scm.Sys
{
    public class Period
    {
        public DateTime? From { get; private set; }
        public DateTime? To { get; private set; }

        public Period() { }
        public Period(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue && to.Value < from.Value)
                throw new NotSupportedException($"Period does not support to < from: [{from.Value};{to.Value})");
            From = from;
            To = to;
        }

        public static Period Infinite() => new Period();
        public static Period Starting(DateTime? at) => new Period(at, null);
        public static Period Ending(DateTime? at) => new Period(null, at);
    }

    public static class PeriodExtensions
    {
        public static Period StartAt(this Period old, DateTime? newStart)
            => new Period(newStart, old.To);
    }
}