using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Scm.Rx.Tests
{
    public static class PublishReplayExtensions
    {
        public static IObservable<T> PublishReplay<T>(this IObservable<T> source, int count)
            => source.Multicast(new ReplaySubject<T>(1)).RefCount();
    }
}