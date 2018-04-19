using System;

namespace Scm.Sys
{
    public class Period
    {
        public Period()
        {
        }

        public Period(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue && to.Value < from.Value)
                throw new NotSupportedException($"Period does not support to < from: [{from.Value};{to.Value})");
            From = from;
            To = to;
        }

        public DateTime? From { get; }
        public DateTime? To { get; }

        public static Period Infinite() => new Period();
        public static Period Starting(DateTime? at) => new Period(at, null);
        public static Period Ending(DateTime? at) => new Period(null, at);
    }
}