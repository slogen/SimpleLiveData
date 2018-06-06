using System;
using System.Runtime.CompilerServices;
using Scm.Rx;
using Scm.Sys;
using Xunit.Abstractions;

namespace DataSys.App.Tests.Test
{
    public static class TestHereTracerExtensions
    {
        public static IObservable<T> TraceTest<T>(this IObservable<T> source, ITestOutputHelper output,
            bool? enabled = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
         => source.Wrap(new TestHereTracer(output, CallerInfo.Here(callerMemberName, callerFilePath, callerLineNumber), enabled).Trace);
    }
}