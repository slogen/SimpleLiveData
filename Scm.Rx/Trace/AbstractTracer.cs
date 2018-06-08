using System;
using System.Configuration;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace Scm.Rx.Trace
{
    public abstract class AbstractTracer
    {
        public static bool DefaultEnabled =
            bool.Parse(ConfigurationManager.AppSettings[typeof(AbstractTracer).FullName] ??
#if DEBUG
                       "true"
#else
          "false"
#endif
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected AbstractTracer()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnNext<T>(IObservable<T> source, T next);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnError<T>(IObservable<T> source, Exception exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnCompleted<T>(IObservable<T> source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool IsEnabled() => DefaultEnabled;

        public IObservable<T> Trace<T>(IObservable<T> source)
            => IsEnabled()
                ? source.Do(x => OnNext(source, x), ex => OnError(source, ex), () => OnCompleted(source))
                : source;
    }
}