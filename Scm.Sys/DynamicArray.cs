using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Sys
{
    public static class DynamicArray
    {
        public static Array AsArray(this ICollection collection, Type elementType)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));
            if (collection is Array array && array.GetType().GetElementType() == elementType)
                return array;
            var a = Array.CreateInstance(elementType, collection.Count);
            collection.CopyTo(a, 0);
            return a;
        }
        public static Array AsArray(this IEnumerable enumerable, Type elementType)
        {
            if (!(enumerable is ICollection c))
                c = enumerable.AsList(elementType);
            return c.AsArray(elementType);
        }
        public static async Task<T[]> AsArrayAsync<T>(this IAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
         => await enumerable.ToArray(cancellationToken).ConfigureAwait(false);

        public static async Task<Array> ObjectAsArray(this object enumerableOrAsyncEnumerable, CancellationToken cancellationToken)
        {
            if (enumerableOrAsyncEnumerable is null)
                return null;
            var elementType = enumerableOrAsyncEnumerable.ElementType();
            if (enumerableOrAsyncEnumerable is IEnumerable enumerable)
                return enumerable.AsArray(elementType);
            if (elementType != null)
                return await enumerableOrAsyncEnumerable.DynamicAsArrayAsync(cancellationToken)
                    .ConfigureAwait(false);
            throw new NotSupportedException($"Object {enumerableOrAsyncEnumerable} AsArray not supported");
        }

        private static async Task<Array> AsArrayAsyncReflectionHack<T>(this IAsyncEnumerable<T> enumerable,
            CancellationToken cancellationToken)
            => await enumerable.AsArrayAsync(cancellationToken).ConfigureAwait(false);
        private static Task<Array> DynamicAsArrayAsync(this object asyncEnumerable, CancellationToken cancellationToken)
        {
            var bakedMethodInfo = AsyncEnumerableAsArray.MakeGenericMethod(asyncEnumerable.ElementType());
            var result = bakedMethodInfo.Invoke(null,
                new[]
                {
                    asyncEnumerable, cancellationToken
                });
            return (Task<Array>)result;
        }

        private static MethodInfo _asyncEnumerableAsArray;
        private static MethodInfo AsyncEnumerableAsArray =>
            _asyncEnumerableAsArray ?? (_asyncEnumerableAsArray = FindAsyncEnumerableAsArray());

        private static MethodInfo FindAsyncEnumerableAsArray()
        {

            var mis = typeof(DynamicArray)
                .GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic)
                .Where(m => m.Name == nameof(AsArrayAsyncReflectionHack))
                .ToList();
            var mis3 = mis.Where(mi => mi.GetParameters().Skip(1).Select(x => x.ParameterType).SequenceEqual(new[]
                {typeof(CancellationToken)}))
                .ToList();
            return mis3.First();
        }

    }
}
