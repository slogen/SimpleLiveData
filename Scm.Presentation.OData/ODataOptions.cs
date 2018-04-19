using System.Diagnostics.CodeAnalysis;
using Scm.Rx;

namespace Scm.Presentation.OData
{
    public class ODataOptions : IODataOptions
    {
        public static IODataOptions Default { get; set; } = DefaultODataOptions.Default;
        public IQueryTimeSpan QueryTimeSpan { get; set; }
        public IOdataApplyOptions ApplyOptions { get; set; }
        public ISchedulerProvider SchedulerProvider { get; set; }

        private class DefaultODataOptions : IODataOptions
        {
            private DefaultODataOptions()
            {
            }

            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification =
                "DefaultInstance is unrelated")]
            public static DefaultODataOptions Default { get; } = new DefaultODataOptions();

            public IQueryTimeSpan QueryTimeSpan => null;
            public IOdataApplyOptions ApplyOptions => null;
            public ISchedulerProvider SchedulerProvider => null;
        }
    }
}