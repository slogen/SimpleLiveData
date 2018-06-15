using System;

namespace Scm.Sys
{
    public class MissingDisposeEventArgs : EventArgs
    {
        public MissingDisposeEventArgs(WeakReference obj)
        {
            Object = obj;
        }

        public WeakReference Object { get; }
    }
}