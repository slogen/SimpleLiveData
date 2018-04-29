using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Scm.Sys;

namespace SimpleLiveData.App.DataModel
{
    public class Data : IData
    {
        public Data()
        {
        }

        public Data(Guid installationId, Guid signalId, DateTime timeStamp, float value)
        {
            InstallationId = installationId;
            SignalId = signalId;
            TimeStamp = timeStamp;
            Value = value;
        }

        public Installation Installation { get; set; }

        public Signal Signal { get; set; }

        [ForeignKey(nameof(Installation))] public Guid InstallationId { get; set; }

        [ForeignKey(nameof(Signal))] public Guid SignalId { get; set; }
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }

        public static class Comparer
        {
            public static IComparer<Data> ByTimeThenInstallationThenSignal =>
                ByTimeThenInstallationThenSignalComparer.Default;

            private class ByTimeThenInstallationThenSignalComparer : NullDispatchComparer<Data>
            {
                private ByTimeThenInstallationThenSignalComparer()
                {
                }

                public static IComparer<Data> Default { get; } = new ByTimeThenInstallationThenSignalComparer();

                protected override int CompareNonNull(Data x, Data y)
                {
                    var d = Comparer<DateTime>.Default.Compare(x.TimeStamp, y.TimeStamp);
                    if (d != 0)
                        return d;
                    d = Comparer<Guid>.Default.Compare(x.InstallationId, y.InstallationId);
                    return d != 0 ? d : Comparer<Guid>.Default.Compare(x.SignalId, y.SignalId);
                }
            }
        }
    }
}