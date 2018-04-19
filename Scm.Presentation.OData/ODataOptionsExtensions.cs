using Scm.Rx;

namespace Scm.Presentation.OData
{
    public static class ODataOptionsExtensions
    {
        public static IQueryTimeSpan QueryTimeSpan(this IODataOptions oDataOptions)
            => oDataOptions?.QueryTimeSpan ?? ODataOptions.Default.QueryTimeSpan;

        public static IOdataApplyOptions ApplyOptions(this IODataOptions oDataOptions)
            => oDataOptions?.ApplyOptions ?? ODataOptions.Default.ApplyOptions;

        public static ISchedulerProvider SchedulerProvider(this IODataOptions oDataOptions)
            => oDataOptions?.SchedulerProvider ?? ODataOptions.Default.SchedulerProvider;
    }
}