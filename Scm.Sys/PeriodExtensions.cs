using System;

namespace Scm.Sys
{
    public static class PeriodExtensions
    {
        public static Period StartAt(this Period old, DateTime? newStart)
            => new Period(newStart, old.To);
    }
}