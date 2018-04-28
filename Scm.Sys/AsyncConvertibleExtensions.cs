using System.Threading;
using System.Threading.Tasks;
using Type = System.Type;

namespace Scm.Sys
{
    public static class AsyncConvertibleExtensions
    {
        public static async Task<T> Convert<T>(this IAsyncConvertible convertible, CancellationToken cancellationToken)
            => (T) await convertible.Convert(typeof(T), cancellationToken);

        public static async Task<T> Convert<T>(this IAsyncConvertible convertible, T witness,
            CancellationToken cancellationToken)
            => await convertible.Convert<T>(cancellationToken);

        public static async Task<object> Convert(this Task<IAsyncConvertible> convertibleTask, Type targetType,
            CancellationToken cancellationToken)
            => await (await convertibleTask.ConfigureAwait(false)).Convert(targetType, cancellationToken)
                .ConfigureAwait(false);

        public static async Task<T> Convert<T>(this Task<IAsyncConvertible> convertibleTask,
            CancellationToken cancellationToken)
            => (T) await convertibleTask.Convert(typeof(T), cancellationToken);

        public static async Task<T> Convert<T>(this Task<IAsyncConvertible> convertible, T witness,
            CancellationToken cancellationToken)
            => await convertible.Convert<T>(cancellationToken);
    }
}