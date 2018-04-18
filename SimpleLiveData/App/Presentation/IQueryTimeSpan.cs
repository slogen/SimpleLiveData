using System;

namespace SimpleLiveData.App.Presentation
{
    public interface IQueryTimeSpan
    {
        TimeSpan? QueryTimeout { get; }
    }
}