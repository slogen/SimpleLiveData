using System;

namespace Scm.Sys
{
    public static class DateAndTimeTruncationExtensions
    {
        public static DateTimeKind DefaultDateTimeKind { get; set; } = DateTimeKind.Utc;
        public static DateTime Epoch(this DateTimeKind? kind) => (kind ?? DefaultDateTimeKind).Epoch();
        public static DateTime Epoch(this DateTimeKind kind)
            => new DateTime(2000, 1, 1, 0, 0, 0, kind);
        public static DateTime TruncateTo(this DateTime dateTime, TimeSpan span, DateTime? epoch = null)
        {
            var diff = dateTime - (epoch ?? dateTime.Kind.Epoch());
            var excess = diff.Ticks % span.Ticks;
            if (excess == 0)
                return dateTime;
            return dateTime.AddTicks(-excess);
        }
        public static DateTimeOffset DefaultDateTimeOffsetEpoch { get; set; } = new DateTimeOffset(DefaultDateTimeKind.Epoch());

        public static DateTimeOffset TruncateTo(this DateTimeOffset dateTimeOffset, TimeSpan span, DateTimeOffset? epoch = null)
        {
            var diff = dateTimeOffset - (epoch ?? DefaultDateTimeOffsetEpoch);
            var excess = diff.Ticks % span.Ticks;
            if (excess == 0)
                return dateTimeOffset;
            return dateTimeOffset.AddTicks(-excess);
        }
    }
}
