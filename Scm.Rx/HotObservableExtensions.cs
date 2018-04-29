using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Scm.Rx
{
    public static class HotObservableExtensions
    {
        public static IObservable<long> HotCount<TSource>(this IObservable<TSource> source)
        {
            var obs = source
                .Select((e, i) => new {e, i = i + 1L})
                .Select(x => x.i)
                .Multicast(new BehaviorSubject<long>(0L));
            obs.Connect();
            return obs;
        }
    }
}