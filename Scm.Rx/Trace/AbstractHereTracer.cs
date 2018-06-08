using System;
using System.Runtime.CompilerServices;
using Scm.Sys;

namespace Scm.Rx.Trace
{
    public abstract class AbstractHereTracer : AbstractTracer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected AbstractHereTracer(
            ICallerInfo callerInfo,
            bool? enabled = null)
        {
            CallerInfo = callerInfo;
            Enabled = enabled;
        }

        public bool? Enabled { get; set; }

        public ICallerInfo CallerInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        protected abstract void WriteLine(string format, params object[] args);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual object OnNextObject<T>(IObservable<T> source, T next)
            => next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsEnabled() => Enabled ?? base.IsEnabled();

        protected override void OnNext<T>(IObservable<T> source, T next)
            => WriteLine("{0}:{1}", CallerInfo, OnNextObject(source, next));

        protected override void OnError<T>(IObservable<T> source, Exception exception)
            => WriteLine("{0}: Error {1}", CallerInfo, exception);

        protected override void OnCompleted<T>(IObservable<T> source)
            => WriteLine("{0}: Completed", CallerInfo);
    }
}