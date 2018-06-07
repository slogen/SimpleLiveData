using System.Threading;
using DataSys.App.DataAccess;

namespace DataSys.App.Tests.Support
{
    public class TestAppUnitOfWorkFactory : AppUnitOfWorkFactory
    {
        private static long _id;

        public TestAppUnitOfWorkFactory()
        {
            Id = Interlocked.Increment(ref _id);
        }

        public long Id { get; }
        protected override string DbContextName => $"TestAppDb{Id}";
    }
}