using Microsoft.AspNet.OData.Query;

namespace Scm.Presentation.OData
{
    public class ODataApplyOptions: IOdataApplyOptions
    {
        public static bool DefaultOrdered { get; set; } = false;
        private class DefaultOdataApplyOptions: IOdataApplyOptions
        {
            public static DefaultOdataApplyOptions Default { get; } = new DefaultOdataApplyOptions();
            public ODataQuerySettings QuerySettings => new ODataQuerySettings {PageSize = 10000, HandleNullPropagation = HandleNullPropagationOption.False};

            public AllowedQueryOptions? IgnoreQueryOptions(bool ordered)
                => ordered ? AllowedQueryOptions.None : AllowedQueryOptions.OrderBy;
        }
        public static IOdataApplyOptions DefaultInstance { get; set; } = DefaultOdataApplyOptions.Default;
        public ODataQuerySettings QuerySettings { get; set; }

        public virtual AllowedQueryOptions? IgnoreQueryOptions(bool ordered)
            => DefaultInstance.IgnoreQueryOptions(ordered);
    }
}