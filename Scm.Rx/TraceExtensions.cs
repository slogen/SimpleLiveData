using Scm.Sys;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Scm.Rx
{

    public static class TraceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IObservable<T> Trace<T>(
            this IObservable<T> source,
            TextWriter writer,
            ICallerInfo callerInfo,
            bool? enabled = null)
            => source.Wrap(new TextWriterHereTracer(writer, callerInfo, enabled).Trace);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IObservable<T> TraceHere<T>(
            this IObservable<T> source,
            TextWriter writer = null,
            bool? enabled = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            => source.Trace(writer ?? Console.Error, CallerInfo.Here(callerMemberName, callerFilePath, callerLineNumber), enabled);
        public static IObservable<T> Wrap<T>(
            this IObservable<T> source,
            Func<IObservable<T>, IObservable<T>> wrapper)
        {
            if (wrapper == null)
                return source;
            return wrapper(source);
        }
    }
}
