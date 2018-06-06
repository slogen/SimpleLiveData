using Scm.Sys;
using System;
using System.Runtime.CompilerServices;

namespace Scm.Rx
{
    public abstract class AbstractHereTracer : AbstractTracer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AbstractHereTracer(
            ICallerInfo callerInfo, 
            bool? enabled = null)
        {
            CallerInfo = callerInfo;
            Enabled = enabled;
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

        public bool? Enabled { get; set; }
        public ICallerInfo CallerInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}
