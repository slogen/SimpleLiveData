using System;

namespace Scm.Sys
{
    public static class DateTimeTruncateExtensions
    {
        public static DateTime Truncate(this DateTime dateTime, TimeSpan span, DateTime? epoch = null)
        {
            var epochTicks = epoch?.Ticks ?? 0;
            var dtTicks = dateTime.Ticks;
            var tsTicks = span.Ticks;
            var diff = dtTicks - epochTicks;
            var truncate = diff % tsTicks;
            return new DateTime(dtTicks - truncate);
        }
    }
}