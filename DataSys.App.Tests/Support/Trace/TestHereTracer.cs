using System;
using Newtonsoft.Json;
using Scm.Rx.Trace;
using Scm.Sys;
using Xunit.Abstractions;

namespace DataSys.App.Tests.Support.Trace
{
    public class TestHereTracer : AbstractHereTracer
    {
        public TestHereTracer(ITestOutputHelper writer, ICallerInfo callerInfo, bool? enabled = null) : base(callerInfo,
            enabled)
        {
            Writer = writer;
        }

        public ITestOutputHelper Writer { get; }

        // on by default in tests
        public override bool IsEnabled() => Enabled ?? true;

        protected override object OnNextObject<T>(IObservable<T> source, T next)
        {
            return JsonConvert.SerializeObject(base.OnNextObject(source, next));
        }

        protected override void WriteLine(string format, params object[] args)
            => Writer.WriteLine(format, args);
    }
}