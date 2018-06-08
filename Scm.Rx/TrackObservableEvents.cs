using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace Scm.Rx.Tests
{
    public class TrackObservableEvents<T>: IObservable<T>
    {
        public IObservable<T> Parent { get; }
        public IList<Notification<T>> Events { get; }
        public TrackObservableEvents(IObservable<T> parent, IList<Notification<T>> events = null)
        {
            Parent = parent;
            Events = events ?? new List<Notification<T>>();
        }
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Parent.Materialize().Do(Events.Add).Dematerialize().Subscribe(observer);
        }
    }
    public static class TrackObservableEvents
    {
        public static TrackObservableEvents<T> TrackEvents<T>(this IObservable<T> parent, IList<Notification<T>> events = null)
            => new TrackObservableEvents<T>(parent, events);
    }
}