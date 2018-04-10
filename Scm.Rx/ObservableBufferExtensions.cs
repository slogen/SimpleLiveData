using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Scm.Rx
{
    public static class ObservableBufferExtensions
    {
        public static IObservable<IList<T>> Buffer<T>(
            this IObservable<T> source, TimeSpan? timeSpan, int? count, IScheduler scheduler)
        {
            if (!timeSpan.HasValue)
                return count.HasValue ? source.Buffer(count.Value) : source.ToList();
            if (!count.HasValue)
                return source.ToList();
            return scheduler is null
                ? source.Buffer(timeSpan.Value, count.Value)
                : Observable.Buffer(source, timeSpan.Value, count.Value, scheduler);
        }
    }
}