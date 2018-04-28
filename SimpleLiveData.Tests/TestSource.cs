using System;

namespace SimpleLiveData.Tests
{
    public class TestSource :
        AbstractTestSource
    {
        public override TimeSpan ObserveIntervalSpan => TimeSpan.FromSeconds(60);
    }
}