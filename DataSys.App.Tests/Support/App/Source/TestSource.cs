using System;
using DataSys.App.DataAccess;

namespace DataSys.App.Tests.Support.App.Source
{
    public class TestSource :
        AbstractTestSource
    {
        public TestSource(IAppUnitOfWorkFactory appUnitOfWorkFactory)
        {
            AppUnitOfWorkFactory = appUnitOfWorkFactory;
        }

        public override TimeSpan ObserveIntervalSpan => TimeSpan.FromSeconds(60);

        public IAppUnitOfWorkFactory AppUnitOfWorkFactory { get; }

        public override IAppUnitOfWork UnitOfWork() => AppUnitOfWorkFactory.UnitOfWork();
    }
}