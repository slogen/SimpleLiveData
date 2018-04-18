using System;
using System.Diagnostics.CodeAnalysis;

namespace SimpleLiveData.App.Presentation
{
    public static class QueryTimeSpanExtensions
    {
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException", Justification = "The default implementation must provide a value")]
        public static TimeSpan QueryTimeSpan(this IQueryTimeSpan queryTimeSpan)
            => queryTimeSpan?.QueryTimeout ?? Presentation.QueryTimeSpan.DefaultInstance.QueryTimeout.Value;
    }
}