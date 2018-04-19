using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.OData.Query;

namespace Scm.Presentation.OData
{
    public static class OdataApplyOptionsExtensions
    {
        public static ODataQuerySettings QuerySettings(this IOdataApplyOptions odataApplyOptions)
            => odataApplyOptions?.QuerySettings ?? ODataApplyOptions.DefaultInstance.QuerySettings;

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException", Justification =
            "DefaultInstance must provide value")]
        [SuppressMessage("ReSharper", "TailRecursiveCall", Justification = "Tail-call is to another object")]
        public static AllowedQueryOptions IgnoredQueryOptions(this IOdataApplyOptions odataApplyOptions,
            bool? ordered = null)
            => odataApplyOptions?.IgnoreQueryOptions(ordered ?? ODataApplyOptions.DefaultOrdered)
               ?? ODataApplyOptions.DefaultInstance.IgnoredQueryOptions(ordered ?? ODataApplyOptions.DefaultOrdered);
    }
}