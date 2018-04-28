using System;

namespace Scm.Sys
{
    public static class TimeMultiplierExtensions
    {
        public static bool DefaultThrowOnOverflow = false;

        /// <summary>
        /// Exactly multiply <paramref name="span"/> by <paramref name="multiplier"/>.
        /// 
        /// Will return <see cref="TimeSpan.MinValue"/> or <see cref="TimeSpan.MaxValue"/> respectively when <paramref name="throwOnOverflow"/> is not set or throw otherwise
        /// </summary>
        /// <exception cref="OverflowException">If <paramref name="throwOnOverflow"/> is set and the multiplication overflows</exception>
        public static TimeSpan Times(this TimeSpan span, long multiplier, bool? throwOnOverflow)
        {
            decimal ticks = span.Ticks * multiplier;
            if (throwOnOverflow ?? DefaultThrowOnOverflow)
                return TimeSpan.FromTicks((long) ticks);
            return ticks <= TimeSpan.MinValue.Ticks ? TimeSpan.MinValue
                : ticks >= TimeSpan.MaxValue.Ticks ? TimeSpan.MaxValue
                : TimeSpan.FromTicks((long) ticks);
        }

        public static TimeSpan Times(this TimeSpan span, long multiplier) =>
            span.Times(multiplier, throwOnOverflow: null);

        public static TimeSpan Times(this TimeSpan span, int multiplier) =>
            span.Times(multiplier, throwOnOverflow: null);

        public static TimeSpan Times(this TimeSpan span, short multiplier) =>
            span.Times(multiplier, throwOnOverflow: null);
    }
}