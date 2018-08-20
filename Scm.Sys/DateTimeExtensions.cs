using System;
using System.Runtime.CompilerServices;

namespace Scm.Sys
{
    public static class DateTimeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime LimitMax(this DateTime left, DateTime right)
            => left <= right ? left : right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime LimitMin(this DateTime left, DateTime right)
            => left < right ? right : left;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan LimitMax(this TimeSpan left, TimeSpan right)
            => left <= right ? left : right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan LimitMin(this TimeSpan left, TimeSpan right)
            => left < right ? right : left;
    }
}
