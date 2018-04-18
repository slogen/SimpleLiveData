using System;

namespace Scm.Presentation.OData
{
    public interface IQueryTimeSpan
    {
        TimeSpan? QueryTimeout { get; }
    }
}