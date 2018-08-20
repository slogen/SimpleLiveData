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
        public object SyncRoot { get; set; } = new object();
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

        public static IClock DefaultClock { get; set; } = SystemClockUtc.Default;
        public static TimeSpan? DefaultTimeOut = null;
        public static TimeSpan DefaultCheckInterval = TimeSpan.FromMilliseconds(50);

        /// <summary>
        /// Advance the clock <paramref name="amount"/>, or until the next pending event (if there is one)
        /// </summary>
        public IEnumerable<Task> Advance(TimeSpan? amount = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (SyncRoot)
            {
                _now = (amount == default(TimeSpan?) ? _pending.Values.FirstOrDefault()?.At : Now() + amount.Value) ?? _now;
                return ActivatePending(cancellationToken);
            }
        }

        private IEnumerable<Task> ActivatePending(CancellationToken cancellationToken)
        {
            Contract.Assert(Monitor.IsEntered(SyncRoot));
            var now = Now();
            var lst = _pending.Values.TakeWhile(pt => pt.At <= now).ToList();
            return lst.Select(p =>
            {
                p.Activate();
                return p.WaitActivationAsync(cancellationToken);
            });
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
            private readonly ManualResetEvent _activated = new ManualResetEvent(false);
            public void Activate() => _activated.Set();
            public Task WaitActivationAsync(CancellationToken cancellationToken) => _activated.WaitAsync(cancellationToken);
            public void WaitActivation() => _activated.WaitOne();

            public int CompareTo(Pending other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (other is null) return 1;
                var diff = At.CompareTo(other.At);
                return diff != 0 ? diff : Id.CompareTo(other.Id);
            }
        }
        private readonly SortedDictionary<long, Pending> _pending = new SortedDictionary<long, Pending>();

        protected virtual Pending FindPending(TimeSpan span)
        {
            lock (SyncRoot)
            {
                if (span <= TimeSpan.Zero)
                    return null;
                var p = new Pending(Now() + span);
                _pending.Add(p.Id, p);
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
                await pending.WaitActivationAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                lock (SyncRoot)
                    _pending.Remove(pending.Id, out var _);
            }
        }
        public void Sleep(TimeSpan span)
        {
            var pending = FindPending(span);
            if (pending == null)
                return;
            try
            {
                pending.WaitActivation();
            }
            finally
            {
                lock (SyncRoot)
                    _pending.Remove(pending.Id, out var _);
            }
        }
    }
}
