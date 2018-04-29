using System;

namespace SimpleLiveData.Tests.Support
{
    public class TestSource :
        AbstractTestSource
    {
        public override TimeSpan ObserveIntervalSpan => TimeSpan.FromSeconds(60);
    }
}