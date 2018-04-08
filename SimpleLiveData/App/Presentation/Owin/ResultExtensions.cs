using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLiveData.App.Presentation.Owin
{
    public static class ResultExtensions {
        public static async Task<IResult<T>> ToResult<T>(this IObservable<T> source, CancellationToken cancellationToken)
        {
            return new Result<T>(await source.ToList().ToTask(cancellationToken).ConfigureAwait(false));
        }
    }
}