using System;

namespace DataSys.App.Tests.Support
{
    public class TestSource :
        AbstractTestSource
    {
        public override TimeSpan ObserveIntervalSpan => TimeSpan.FromSeconds(60);
    }
}