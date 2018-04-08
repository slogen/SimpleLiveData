using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Scm.DataAccess;
using Scm.DataAccess.Qbservable;
using SimpleLiveData.App.DataAccess;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage
{
    public class MeetUnitOfWork : CommitCountingUnitOfWork, ISomeUnitOfWork
    {
        readonly CancellationTokenSource _dispose = new CancellationTokenSource();
        public IMeet<A> Source { get; }
        public ITrackingMeet<A> LocalMeet { get; }

        IMeet<A> ISomeUnitOfWork.A { get; }
        protected MeetUnitOfWork(IMeet<A> source, ITrackingMeet<A> localMeet)
        {
            Source = source;
            LocalMeet = LocalMeet;
        }
        public IMeet<A> A => LocalMeet;

        // TODO: more separation on where the tracking is
        public MeetUnitOfWork(IMeet<A> source, CancellationTokenSource cancel) :
            this(source, new TrackingMeet<A, List<A>>(source, new List<A>(), cancel.Token)) {
            _dispose = cancel;
        } 
        protected override async Task CommitAsyncOnce(CancellationToken cancellationToken)
        {
            await Source.Add(LocalMeet.Track.ToObservable()).ToTask(cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_dispose.IsCancellationRequested)
                _dispose.Cancel();
            _dispose.Dispose();
        }
    }
}