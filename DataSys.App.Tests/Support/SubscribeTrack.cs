using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace DataSys.App.Tests.Support
{
    public class SubscribeTrack<T> : IObservable<T>
    {
        private readonly BehaviorSubject<long> _subcriptions = new BehaviorSubject<long>(0);
        private long _subscribeCount;

        public SubscribeTrack(IObservable<T> observable)
        {
            Observable = observable.Finally(_subcriptions.OnCompleted);
        }

        public IObservable<T> Observable { get; }
        public IObservable<long> Subscribtions => _subcriptions.AsObservable();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = Observable.Subscribe(observer);
            var id = Interlocked.Increment(ref _subscribeCount);
            _subcriptions.OnNext(id);
            return Disposable.Create(() => { _subcriptions.OnNext(-id); });
        }
    }
}