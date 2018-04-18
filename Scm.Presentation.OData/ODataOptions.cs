using System.Diagnostics.CodeAnalysis;
using Scm.Rx;

namespace Scm.Presentation.OData
{
    public class ODataOptions: IODataOptions
    {
        private class DefaultODataOptions: IODataOptions
        {
            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "DefaultInstance is unrelated")]
            public static DefaultODataOptions Default { get; } = new DefaultODataOptions();
            private DefaultODataOptions() { }
            public IQueryTimeSpan QueryTimeSpan => null;
            public IOdataApplyOptions ApplyOptions => null;
            public ISchedulerProvider SchedulerProvider => null;
        }

        public static IODataOptions Default { get; set; } = DefaultODataOptions.Default;
        public IQueryTimeSpan QueryTimeSpan { get; set; }
        public IOdataApplyOptions ApplyOptions { get; set; }
        public ISchedulerProvider SchedulerProvider { get; set; }
    }
}