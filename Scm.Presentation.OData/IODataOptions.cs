using Scm.Rx;

namespace Scm.Presentation.OData
{
    public interface IODataOptions
    {
        IQueryTimeSpan QueryTimeSpan { get; }
        IOdataApplyOptions ApplyOptions { get; }
        ISchedulerProvider SchedulerProvider { get; }
    }
}