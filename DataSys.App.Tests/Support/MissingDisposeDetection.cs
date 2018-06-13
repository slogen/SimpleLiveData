using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace DataSys.App.Tests.Support
{
    public abstract class MissingDisposeDetection: IDisposable
    {
        private static long _undisposedCount;
        private static readonly ISubject<WeakReference> MissingDisposeSubject = new Subject<WeakReference>();
        public static IObservable<WeakReference> MissingDispose => MissingDisposeSubject.AsObservable();

        private static void NotifyMissingDispose(WeakReference instance)
        {
            Interlocked.Increment(ref _undisposedCount);
            MissingDisposeSubject.OnNext(instance);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                NotifyMissingDispose(new WeakReference(this));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}