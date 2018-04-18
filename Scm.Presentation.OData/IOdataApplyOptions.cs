using Microsoft.AspNet.OData.Query;

namespace Scm.Presentation.OData
{
    public interface IOdataApplyOptions
    {
        ODataQuerySettings QuerySettings { get; }
        AllowedQueryOptions? IgnoreQueryOptions(bool ordered);
    }
}