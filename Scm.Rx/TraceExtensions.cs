using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Scm.Rx.Trace;
using Scm.Sys;

namespace Scm.Rx
{
    public static class TraceExtensions
    {
        public static Func<IObservable<T>, IObservable<T>> DefaultWrapper<T>(ICallerInfo callerInfo, bool? enabled = null)
        {
            Func<IObservable<T>, IObservable<T>> wrap = o => o;
            if (Debugger.IsLogging())
            {
                var wrap1 = wrap;
                wrap = o => new ActionHereTracer((f, a) => Debug.WriteLine(f, a), callerInfo, enabled).Trace(wrap1(o));
            }

            {
                var wrap1 = wrap;
                wrap = o => new TextWriterHereTracer(Console.Error, callerInfo, enabled).Trace(wrap1(o));
            }
            return wrap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Pass callerinfo to target")]
        public static IObservable<T> TraceHere<T>(
            this IObservable<T> source,
            bool? enabled = null,
            Func<ICallerInfo, bool?, Func<IObservable<T>, IObservable<T>>> target = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var here = CallerInfo.Here(callerMemberName, callerFilePath, callerLineNumber);
            return source.Wrap(target == null ? DefaultWrapper<T>(here, enabled) : target(here, enabled));
        }

        public static IObservable<T> Wrap<T>(
            this IObservable<T> source,
            Func<IObservable<T>, IObservable<T>> wrapper) 
            => wrapper == null ? source : wrapper(source);
    }
}