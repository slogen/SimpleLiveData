using System;
using System.Diagnostics.CodeAnalysis;

namespace Scm.Presentation.OData
{
    public static class QueryTimeSpanExtensions
    {
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException", Justification = "The default implementation must provide a value")]
        public static TimeSpan QueryTimeSpan(this IQueryTimeSpan queryTimeSpan)
            => queryTimeSpan?.QueryTimeout ?? OData.QueryTimeSpan.DefaultInstance.QueryTimeout.Value;
    }
}