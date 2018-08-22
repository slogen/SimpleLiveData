using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using Scm.Sys;

namespace DataSys.App.Tests.Support.App.Source
{
    public abstract class AbstractTestSource
    {
        protected TimeSpan InstallationBeginInterval = TimeSpan.FromDays(1);
        protected DateTime StartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public abstract TimeSpan ObserveIntervalSpan { get; }
        public static Guid InstallationNameSpace { get; } = Guid.Parse("7c73ec21-a7d9-4bb3-9802-b7193abba077");
        public static Guid SignalNameSpace { get; } = Guid.Parse("0acb31aa-6075-4871-b7a0-2150bac625b5");

        public abstract IAppUnitOfWork UnitOfWork();

        protected Installation Installation(int i)
        {
            var name = $"I{i}";
            var uuid = InstallationNameSpace.Namespace(name);
            return new Installation(uuid, name,
                Period.Starting(StartTime.Add(InstallationBeginInterval.Multiply(i))));
        }

        protected Signal Signal(int i)
        {
            var name = $"S{i}";
            var uuid = SignalNameSpace.Namespace(name);
            return new Signal(uuid, name);
        }

        public async Task Prepare(int installations, int signals, CancellationToken cancellationToken)
        {
            using (var uow = UnitOfWork())
            {
                await uow.Repository<Installation>().AddRangeAsync(Enumerable.Range(0, installations).Select(Installation), cancellationToken)
                    .ConfigureAwait(false);
                await uow.Repository<Signal>().AddRangeAsync(Enumerable.Range(0, signals).Select(Signal), cancellationToken)
                    .ConfigureAwait(false);
                await uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public IEnumerable<Installation> Installations
        {
            get
            {
                using (var uow = UnitOfWork())
                    return uow.Installations.Queryable.ToList();
            }
        }
        public IEnumerable<Signal> Signals
        {
            get
            {
                using (var uow = UnitOfWork())
                    return uow.Signals.Queryable.ToList();
            }
        }

        public IObservable<IList<Data>> ProduceData(IObservable<DateTime> times,
            Func<Installation, Signal, DateTime, float> f = null)
        {
            if (f == null)
                f = (i, s, t) =>
                    (float) Math.Sin(0.0 + i.Id.GetHashCode() + s.Id.GetHashCode() + t.Ticks);
            var obs = times.Select(t =>
                    Observable.FromAsync(async ct =>
                        {
                            using (var uow = UnitOfWork())
                            {
                                var data = (
                                        from inst in Installations
                                        from sig in Signals
                                        select new {inst, sig})
                                    .Select(x => new Data(x.inst.Id, x.sig.Id, t, f(x.inst, x.sig, t)))
                                    .ToList();
                                await uow.Repository<Data>().AddRangeAsync(data, ct).ConfigureAwait(false);
                                await uow.SaveChangesAsync(ct);
                                return data;
                            }
                        }))
                        .Concat();
            return obs.Publish().RefCount();
        }
    }
}