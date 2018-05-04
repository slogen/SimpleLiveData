using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Rx
{
    public static class SideEffectLastExtensions
    {
        public static IObservable<T>
            SideEffectLast<T>(this IObservable<T> source, Func<CancellationToken, Task> last) =>
            source
                .Concat(
                    Observable.DeferAsync(
                        async ct =>
                        {
                            await last(ct).ConfigureAwait(false);
                            return Observable.Empty<T>();
                        }));
    }
}