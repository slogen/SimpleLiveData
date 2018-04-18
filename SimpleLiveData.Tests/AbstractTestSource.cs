using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scm.Linq;
using Scm.Sys;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.Tests
{
    public abstract class AbstractTestSource
    {
        public static Guid InstallationNameSpace { get; } = Guid.Parse("7c73ec21-a7d9-4bb3-9802-b7193abba077");
        public static Guid SignalNameSpace { get; } = Guid.Parse("0acb31aa-6075-4871-b7a0-2150bac625b5");
        protected DateTime StartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected TimeSpan InstallationBeginInterval = TimeSpan.FromDays(1);
        protected ConcurrentDictionary<Guid, Installation> Installations;
        protected ConcurrentDictionary<Guid, Signal> Signals;

        protected Installation Installation(int i)
        {
            var name = $"I{i}";
            var uuid = InstallationNameSpace.Namespace(name);
            return Installations.GetOrAdd(uuid,
                _ => new Installation(uuid, name,
                    Period.Starting(StartTime.Add(InstallationBeginInterval.Multiply(i)))));
        }

        protected Signal Signal(int i)
        {
            var name = $"S{i}";
            var uuid = SignalNameSpace.Namespace(name);
            return Signals.GetOrAdd(uuid, _ => new Signal(uuid, name));
        }

        private readonly ISubject<Data> _data = new Subject<Data>();
        public IQbservable<Data> ObserveData => _data.AsQbservable();

        protected void AddData(Data data)
        {
            if (data.Installation == null)
                data.Installation = Installations[data.InstallationId];
            if (data.Signal == null)
                data.Signal = Signals[data.SignalId];
            data.Installation.Data.Add(data);
            data.Signal.Data.Add(data);
            _data.OnNext(data);
        }

        protected void Prepare(int installations, int signals)
        {
            Enumerable.Range(0, installations).Select(Installation).Execute();
            Enumerable.Range(0, signals).Select(Installation).Execute();
        }

        protected void ProduceData(DateTime t, Func<Installation, Signal, float> f = null)
        {
            if (f == null)
                f = (i, s) =>
                    (float) Math.Sin(0.0 + i.Id.GetHashCode() + s.Id.GetHashCode() + t.Ticks);
            foreach (var i in Installations.Values)
            foreach (var s in Signals.Values)
                AddData(new Data(i.Id, s.Id, t, f(i, s)));
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( disposing )
                _data.OnCompleted();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AbstractTestSource()
        {
            Dispose(false);
        }
    }
}