using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Scm.DataAccess.Qbservable;
using Scm.DataAccess.Queryable;
using Scm.Linq;
using Scm.Presentation.OData;
using Scm.Rx;
using Scm.Sys;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;
using SimpleLiveData.App.Hosting;
using Xunit;

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
    public class TestSource:
        AbstractTestSource, 
        IQueryableSource<Installation>, IQbservableSource<Installation>,
        IQueryableSource<Signal>, IQbservableSource<Signal>,
        IObservableSink<Data>,
        IDataUnitOfWork
    {
        public TimeSpan ObserveIntervalSpan { get; } = TimeSpan.FromSeconds(1);

        public TResult Observe<TResult>(Func<IQbservable<Installation>, TResult> f)
            => f(ObserveData.GroupByUntil(d => d.Installation, i => Observable.Interval(ObserveIntervalSpan))
                .SelectMany(g => g.ToList().Select(l => new Installation(g.Key) {Data = l})));
        public TResult Query<TResult>(Func<IQueryable<Installation>, TResult> f)
            => f(Installations.Values.AsQueryable());
        public TResult Observe<TResult>(Func<IQbservable<Signal>, TResult> f)
            => f(ObserveData.GroupByUntil(d => d.Signal, i => Observable.Interval(ObserveIntervalSpan))
                .SelectMany(g => g.ToList().Select(l => new Signal(g.Key) { Data = l })));
        public TResult Query<TResult>(Func<IQueryable<Signal>, TResult> f)
            => f(Signals.Values.AsQueryable());

        public IConnectableObservable<long> Add<TSource>(IObservable<TSource> entities, IScheduler scheduler = null)
            where TSource : Data
            => entities.Do(AddData).Select((x, i) => i + 1L).Publish(0);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            // No-op
            return Task.CompletedTask;
        }

        IQueryableSource<Installation> IDataUnitOfWork.Installations => this;
        IQueryableSource<Signal> IDataUnitOfWork.Signals => this;
    }

    public abstract class AbstractHttpConfigurationFixture : IDisposable
    {
        public virtual IWebHostBuilder Builder =>
            _builder ?? (_builder = ConfigureBuilder(new WebHostBuilder()));
        public virtual TestServer Server => _server ?? (_server = MakeServer());
        public virtual HttpClient Client => _client ?? (_client = MakeClient());
        public Guid Id { get; } = Guid.NewGuid();

        protected abstract IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder);

        protected virtual TestServer MakeServer()
        {
            return new TestServer(Builder);
        }

        protected virtual HttpClient MakeClient()
            => Server.CreateClient();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Internal variables
        private IWebHostBuilder _builder;
        private TestServer _server;
        private HttpClient _client;

        private static long _undisposedCount;
        private static readonly ISubject<Unit> MissingDisposeSubject = new Subject<Unit>();
        public static IObservable<Unit> MissingDispose => MissingDisposeSubject.AsObservable();
        private static void NotifyMissingDispose()
        {
            Interlocked.Increment(ref _undisposedCount);
            MissingDisposeSubject.OnNext(Unit.Default);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                NotifyMissingDispose();
            try
            {
                _client?.Dispose();
            }
            finally
            {
                _server?.Dispose();
            }
        }
        #endregion


        ~AbstractHttpConfigurationFixture()
        {
            Dispose(false);
        }
    }

    [CollectionDefinition("HttpApp")] // Dclare how to share a fully configured Http interface to App
    public abstract class HttpApp : AbstractHttpConfigurationFixture, ICollectionFixture<HttpApp>
    {
        public IScheduler Scheduler = new FastScheduler(1);

        public virtual IDataUnitOfWork DataUnitOfWork(IServiceProvider sp)
        {
            return new TestSource();
        }
        protected override IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
        {
            var provider = new SchedulerProvider
            {
                // Default = Scheduler
            };
            return builder
                .UseStartup<Startup>()
                .ConfigureTestServices(svcs =>
                {
                    svcs.Add(ServiceDescriptor.Scoped(DataUnitOfWork));
                    svcs.Add(ServiceDescriptor.Singleton(typeof(IODataOptions),
                        new ODataOptions
                        {
                            SchedulerProvider = provider
                        }));
                });

        }
    }

    public static class TestExtensions
    {
        public static async Task<string> TestGetAsync(this HttpClient client, string uri)
        {
            var resp = await client.GetAsync(uri).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Error: {resp.StatusCode}\n{await ExtractExplanation(resp)}");
            }

            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private static async Task<string> ExtractExplanation(HttpResponseMessage resp)
        {
            return await resp.Content.ReadAsStringAsync();
        }
    }

    public class FastScheduler : VirtualTimeScheduler<DateTime, double>
    {
        public FastScheduler(double speedup): base(DateTime.UtcNow, Comparer<DateTime>.Default)
        {
            Speedup = speedup;
        }

        public double Speedup { get; }

        protected override DateTime Add(DateTime absolute, double relative)
            => absolute.Add(TimeSpan.FromSeconds(relative));

        protected override DateTimeOffset ToDateTimeOffset(DateTime absolute)
            => new DateTimeOffset(absolute);

        protected override double ToRelative(TimeSpan timeSpan)
            => timeSpan.TotalSeconds / Speedup;
    }

    [Collection("HttpApp")] // Share fully configured app between tests
    public class QueryControllerTest1
    {
        private readonly HttpApp _app;
        public QueryControllerTest1(HttpApp app)
        {
            _app = app;
        }
        protected CancellationToken CancellationToken => CancellationToken.None;
        [Fact]
        public async Task GetApplied()
        {
            var task = _app.Client.TestGetAsync($"/odata/Installation?$filter=contains(Name, '1')");
            //_app.Scheduler.Start();
            var str = await task.ConfigureAwait(false);
            str.Should().Be("foo");
        }
    }
}