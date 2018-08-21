using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    /// <summary>
    /// Clock that is manually advanced.
    /// 
    /// Useful for tests
    /// </summary>
    public class ManualClock : IClock
    {
        public static DateTime DefaultNow = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public object SyncRoot { get; } = new object();
        public ManualClock(DateTime? now = null)
        {
            _now = now ?? DefaultNow;
        }
        private DateTime _now;

        public DateTime Now()
        {
            lock (SyncRoot)
                return _now;
        }

        public long PendingCount()
        {
            lock (SyncRoot)
                return _pending.Count;
        }

        public static TimeSpan? DefaultTimeOut = null;
        public static TimeSpan DefaultCheckInterval = TimeSpan.FromMilliseconds(50);

        /// <summary>
        /// Advance the clock <paramref name="amount"/>, or until the next pending event (if there is one)
        /// </summary>
        public IList<Task> Advance(TimeSpan? amount = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (SyncRoot)
            {
                _now = (amount == default(TimeSpan?) ? _pending.FirstOrDefault()?.At : Now() + amount.Value) ?? _now;
                return ActivatePending(cancellationToken);
            }
        }

        private IList<Task> ActivatePending(CancellationToken cancellationToken)
        {
            Contract.Assert(Monitor.IsEntered(SyncRoot));
            var now = Now();
            var lst = _pending.TakeWhile(pt => pt.At <= now).ToList();
            return lst.Select(p =>
            {
                p.OnActivate();
                return p.Removed.WaitAsync(cancellationToken);
            })
                .ToList();
        }

        protected class Pending : IComparable<Pending>
        {
            private static long _nextId;
            public Pending(DateTime at)
            {
                At = at;
                Id = Interlocked.Increment(ref _nextId);
            }

            public long Id { get; }
            public DateTime At { get; }
            private readonly ManualResetEventSlim _activated = new ManualResetEventSlim(false);
            private readonly ManualResetEventSlim _removed = new ManualResetEventSlim(false);
            public void OnActivate() => _activated.Set();
            public WaitHandle Activated => _activated.WaitHandle;
            public WaitHandle Removed => _removed.WaitHandle;
            public void OnRemove() => _removed.Set();

            public int CompareTo(Pending other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (other is null) return 1;
                var diff = At.CompareTo(other.At);
                return diff != 0 ? diff : Id.CompareTo(other.Id);
            }
        }
        private readonly SortedSet<Pending> _pending = new SortedSet<Pending>();

        protected virtual Pending FindPending(TimeSpan span)
        {
            lock (SyncRoot)
            {
                if (span <= TimeSpan.Zero)
                    return null;
                var p = new Pending(Now() + span);
                _pending.Add(p);
                return p;
            }
        }
        public async Task Delay(TimeSpan span, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = FindPending(span);
            if (pending == null)
                return;
            try
            {
                await pending.Activated.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                lock (SyncRoot)
                    _pending.Remove(pending);
                pending.OnRemove();
            }
        }
        public void Sleep(TimeSpan span)
        {
            var pending = FindPending(span);
            if (pending == null)
                return;
            try
            {
                pending.Activated.WaitOne();
            }
            finally
            {
                lock (SyncRoot)
                    _pending.Remove(pending);
                pending.OnRemove();
            }

            pending.Removed.WaitOne();
        }
    }
}
