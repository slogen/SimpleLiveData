using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Scm.Rx
{
    public static class ObservableExtensions
    {
        public static IObservable<IList<T>> Buffer<T>(
            this IObservable<T> source, TimeSpan? timeSpan, int? count, IScheduler scheduler)
        {
            if (timeSpan.HasValue)
            {
                if (count.HasValue)
                {
                    if (ReferenceEquals(scheduler, null))
                        return source.Buffer(timeSpan.Value, count.Value);
                    else
                        return source.Buffer(timeSpan.Value, count.Value, scheduler);
                }
                else
                {
                    if (ReferenceEquals(scheduler, null))
                        return source.Buffer(timeSpan.Value);
                    else
                        return source.Buffer(timeSpan.Value, scheduler);
                }
            } else
            {
                if (count.HasValue)
                {
                    if (ReferenceEquals(scheduler, null))
                        return source.Buffer(count.Value);
                    else
                        return source.Buffer(count.Value);
                }
                else
                {
                    return source.ToList();
                }
            }
        }
    }
}
