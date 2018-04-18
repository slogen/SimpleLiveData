using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleLiveData.App.DataModel
{
    public class Data
    {
        public Guid InstallationId { get; set; }
        public Installation Installation { get; set; }
        public Guid SignalId { get; set; }
        public Signal Signal { get; set; }
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }

        public Data(Guid installationId, Guid signalId, DateTime timeStamp, float value)
        {
            InstallationId = installationId;
            SignalId = signalId;
            TimeStamp = timeStamp;
            Value = value;
        }

        public static class Comparer
        {

            private class ByTimeThenInstallationThenSignalComparer : NullDispatchComparer<Data>
            {
                public static IComparer<Data> Default { get; }= new ByTimeThenInstallationThenSignalComparer();
                private ByTimeThenInstallationThenSignalComparer() { }
                protected override int CompareNonNull(Data x, Data y)
                {
                    var d = Comparer<DateTime>.Default.Compare(x.TimeStamp, y.TimeStamp);
                    if (d != 0)
                        return d;
                    d = Comparer<Guid>.Default.Compare(x.InstallationId, y.InstallationId);
                    return d != 0 ? d : Comparer<Guid>.Default.Compare(x.SignalId, y.SignalId);
                }
            }

            public static IComparer<Data> ByTimeThenInstallationThenSignal =>
                ByTimeThenInstallationThenSignalComparer.Default;
        }
    }
}