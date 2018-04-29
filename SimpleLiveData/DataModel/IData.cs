using System;

namespace SimpleLiveData.App.DataModel
{
    public interface IData
    {
        Guid InstallationId { get; }
        Guid SignalId { get; }
        DateTime TimeStamp { get; }
        float Value { get; }
    }
}