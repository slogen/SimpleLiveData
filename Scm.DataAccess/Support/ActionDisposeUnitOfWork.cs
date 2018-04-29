using System;

namespace Scm.DataAccess.Support
{
    public class ActionDisposeUnitOfWork : AbstractNonCommittingUnitOfWork
    {
        public ActionDisposeUnitOfWork(Action<bool> onDispose)
        {
            OnDispose = onDispose;
        }

        public Action<bool> OnDispose { get; }

        protected override void Dispose(bool disposing)
        {
            OnDispose?.Invoke(disposing);
        }
    }
}