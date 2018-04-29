using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Scm.DataAccess.Qbservable;
using Scm.DataAccess.Queryable;
using Scm.DataAccess.Support;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;
using SimpleLiveData.App.Hosting;
using SimpleLiveData.Tests.Support;

namespace SimpleLiveData.Tests.Test
{
    public class TestSourceBasedTests : DataAppTests<Startup>
    {
        private TestSource _testSource;
        protected TestSource TestSource => _testSource ?? (_testSource = MakeTestSource());
        protected JsonSerializer JsonSerializer { get; } = new JsonSerializer();
        protected virtual TestSource MakeTestSource() => new TestSource();

        public IDataUnitOfWork DataUnitOfWork(IServiceProvider sp)
        {
            return new TestDataUnitOfWork(this);
        }

        protected override void ConfigureTestServices(IServiceCollection svcs)
        {
            base.ConfigureTestServices(svcs);
            svcs.Add(ServiceDescriptor.Scoped(DataUnitOfWork));
        }

        public class TestDataUnitOfWork : AbstractNonCommittingUnitOfWork, IDataUnitOfWork
        {
            public TestSourceBasedTests Parent;

            public TestDataUnitOfWork(TestSourceBasedTests parent)
            {
                Parent = parent;
            }

            private TestSource TestSource => Parent.TestSource;

            public IQueryableSource<Installation> Installations => TestSource.Installations;

            public IQueryableSource<Signal> Signals => TestSource.Signals;

            public IQbservableSource<Data> Data => TestSource.Data;

            protected override void Dispose(bool disposing)
            {
                //if (disposing)
                //    Parent._testSource?.Dispose();
            }

            protected override Task CommitAsyncOnce(CancellationToken cancellationToken)
            {
                return Task.CompletedTask; // no-op
            }
        }
    }
}