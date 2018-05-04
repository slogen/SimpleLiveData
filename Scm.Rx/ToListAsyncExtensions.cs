using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Rx
{
    public static class ToListAsyncExtensions
    {
        public static Task<IList<T>> ToListAsync<T>(this IObservable<T> source, CancellationToken cancellationToken)
            => source.ToList().ToTask(cancellationToken);
    }
}