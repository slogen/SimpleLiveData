using System;
using System.Threading;

namespace Scm.Sys
{
    public class MissingDisposeEventArgs : EventArgs
    {
        public WeakReference Object { get; }
        public MissingDisposeEventArgs(WeakReference obj) {
            Object = obj;
        }
    }
    public abstract class MissingDisposeDetection: IDisposable
    {
        private static long _undisposedCount;
        public static event EventHandler<MissingDisposeEventArgs> MissingDispose;
        private static void NotifyMissingDispose(WeakReference instance)
        {
            Interlocked.Increment(ref _undisposedCount);
            MissingDispose?.Invoke(null, new MissingDisposeEventArgs(instance));
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