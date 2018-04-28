using System;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.Presentation.SignalR
{
    public class HubData : IData
    {
        public HubData()
        {
        }

        public HubData(Data data)
        {
            InstallationId = data.InstallationId;
            SignalId = data.SignalId;
            TimeStamp = data.TimeStamp;
            Value = data.Value;
        }

        public Guid InstallationId { get; set; }

        public Guid SignalId { get; set; }

        public DateTime TimeStamp { get; set; }

        public float Value { get; set; }
    }
}