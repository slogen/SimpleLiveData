using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DataSys.App.DataModel;
using Scm.DataAccess;
using Scm.DataAccess.Efc2;
using Scm.DataStorage.Efc2;
using Scm.Linq;
using Scm.Sys;

namespace DataSys.App.Tests.Support
{
    public abstract class AbstractTestSource :
        IQueryableSource<Installation>,
        IQbservableSource<Installation>,
        IQueryableSource<Signal>,
        IQbservableSource<Signal>,
        ISink<Data>,
        IQbservableSource<Data>,
        IDisposable
    {
        protected DateTime StartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public abstract TimeSpan ObserveIntervalSpan { get; }
        public static Guid InstallationNameSpace { get; } = Guid.Parse("7c73ec21-a7d9-4bb3-9802-b7193abba077");
        public static Guid SignalNameSpace { get; } = Guid.Parse("0acb31aa-6075-4871-b7a0-2150bac625b5");

        #region Installations

        protected TimeSpan InstallationBeginInterval = TimeSpan.FromDays(1);

        protected ConcurrentDictionary<Guid, Installation> InstallationsById =
            new ConcurrentDictionary<Guid, Installation>();

        protected Installation Installation(int i)
        {
            var name = $"I{i}";
            var uuid = InstallationNameSpace.Namespace(name);
            return InstallationsById.GetOrAdd(uuid,
                _ => new Installation(uuid, name,
                    Period.Starting(StartTime.Add(InstallationBeginInterval.Multiply(i)))));
        }

        public IQueryableSource<Installation> Installations => this;

        public TResult Observe<TResult>(Func<IQbservable<Installation>, TResult> f)
            => f(ObserveData.GroupByUntil(d => d.Installation, i => Observable.Interval(ObserveIntervalSpan))
                .SelectMany(g => g.ToList().Select(l => new Installation(g.Key) {Data = l})));

        public TResult Query<TResult>(Func<IQueryable<Installation>, TResult> f)
            => f(InstallationsById.Values.AsAsyncQueryable());

        #endregion

        #region Signals

        protected ConcurrentDictionary<Guid, Signal> SignalsById = new ConcurrentDictionary<Guid, Signal>();

        protected Signal Signal(int i)
        {
            var name = $"S{i}";
            var uuid = SignalNameSpace.Namespace(name);
            return SignalsById.GetOrAdd(uuid, _ => new Signal(uuid, name));
        }

        public IQueryableSource<Signal> Signals => this;

        public TResult Observe<TResult>(Func<IQbservable<Signal>, TResult> f)
            => f(ObserveData.GroupByUntil(d => d.Signal, i => Observable.Interval(ObserveIntervalSpan))
                .SelectMany(g => g.ToList().Select(l => new Signal(g.Key) {Data = l})));

        public TResult Query<TResult>(Func<IQueryable<Signal>, TResult> f)
            => f(SignalsById.Values.AsAsyncQueryable());

        #endregion

        #region Data

        private readonly ISubject<Data> _data = new Subject<Data>();
        private SubscribeTrack<Data> _dataTrack;
        public SubscribeTrack<Data> DataTracked => _dataTrack ?? (_dataTrack = new SubscribeTrack<Data>(_data));

        public IObservable<int> DataSubscribeCount = new BehaviorSubject<int>(0);
        public IQbservable<Data> ObserveData => _dataTrack.AsQbservable();
        public IQbservableSource<Data> Data => this;

        public IObservable<long> Change(IObservable<IGroupedObservable<EntityChange, Data>> change)
        {
            throw new NotImplementedException();
        }

        public IObservable<long> DynamicChange(IObservable<IGroupedObservable<EntityChange, object>> change)
        {
            throw new NotImplementedException();
        }

        public TResult Observe<TResult>(Func<IQbservable<Data>, TResult> f)
            => f(ObserveData);

        public void AddData(Data data)
        {
            if (data.Installation == null)
                data.Installation = InstallationsById[data.InstallationId];
            if (data.Signal == null)
                data.Signal = SignalsById[data.SignalId];
            data.Installation.Data.Add(data);
            data.Signal.Data.Add(data);
            _data.OnNext(data);
        }

        public void Prepare(int installations, int signals)
        {
            Enumerable.Range(0, installations).Select(Installation).Execute();
            Enumerable.Range(0, signals).Select(Signal).Execute();
        }

        public IEnumerable<Data> ProduceData(Func<Installation, Signal, DateTime, float> f = null,
            DateTime? startAt = null, TimeSpan? interval = null)
        {
            if (f == null)
                f = (i, s, t) =>
                    (float) Math.Sin(0.0 + i.Id.GetHashCode() + s.Id.GetHashCode() + t.Ticks);
            var span = interval ?? TimeSpan.FromSeconds(1);
            var at = (startAt ?? DateTime.UtcNow).Truncate(span);
            for (var i = 0; i < int.MaxValue; ++i)
            {
                var t = at + span.Times(i);
                foreach (var inst in InstallationsById.Values)
                foreach (var sig in SignalsById.Values)
                {
                    var data = new Data(inst.Id, sig.Id, t, f(inst, sig, t));
                    AddData(data);
                    yield return data;
                }
            }
        }

        #endregion

        #region  Dispose

        protected virtual void Dispose(bool disposing)
        {
            _data?.OnCompleted();
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

        #endregion
    }
}