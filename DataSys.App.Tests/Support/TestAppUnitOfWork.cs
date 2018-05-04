using DataSys.App.DataAccess;

namespace DataSys.App.Tests.Support
{
    public class TestAppUnitOfWorkFactory: AppUnitOfWorkFactory
    {
        protected override string DbContextName => "TestAppDb";
    }
}