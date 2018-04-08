using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Scm.DataAccess.Qbservable;
using Scm.DataStorage.Efc2;

namespace SimpleLiveData.App.DataStorage
{
    public class TrackingMeet<TEntity, TTrack> : AbstractTrackingMeet<TEntity, TTrack>
        where TTrack: ICollection<TEntity>
    {
        protected override IObservable<T> CheckDisposed<T>(IObservable<T> source)
        {
            if (CancellationToken.CanBeCanceled)
                source = source.Select(x => { CancellationToken.ThrowIfCancellationRequested(); return x; });
            return base.CheckDisposed(source);
        }
        public TrackingMeet(IMeet<TEntity> source, TTrack track, CancellationToken cancellationToken = default(CancellationToken))
        {
            Source = source;
            Track = track;
            CancellationToken = cancellationToken;
        }

        protected override IMeet<TEntity> Source { get; }
        public override TTrack Track { get; }
        public CancellationToken CancellationToken { get; }
    }
}