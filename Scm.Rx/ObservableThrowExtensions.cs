using System;
using System.Reactive.Linq;

namespace Scm.Rx
{
    public static class ObservableThrowExtensions
    {
        /// <summary>
        /// Throws <paramref name="throwFunc"/>(firstOrDefault(<paramref name="source"/>))
        /// </summary>
        public static IObservable<T> Throw<T, TException>(this IObservable<T> source, Func<T, TException> throwFunc)
            where TException : Exception
            => source.FirstOrDefaultAsync().Select<T, T>(x => throw throwFunc(x));
    }
}