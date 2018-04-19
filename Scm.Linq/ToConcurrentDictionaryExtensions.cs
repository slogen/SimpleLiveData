using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Scm.Linq
{
    public static class ToConcurrentDictionaryExtensions
    {
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keyOf,
            Func<TSource, TValue> valueOf,
            IEqualityComparer<TKey> keyComparer = null,
            int? concurrencyLevel = null)
        {
            return concurrencyLevel.HasValue
                ? new ConcurrentDictionary<TKey, TValue>(
                    concurrencyLevel.Value,
                    source.Select(x => KeyValuePair.Create(keyOf(x), valueOf(x))),
                    keyComparer)
                : new ConcurrentDictionary<TKey, TValue>(
                    source.Select(x => KeyValuePair.Create(keyOf(x), valueOf(x))),
                    keyComparer);
        }
    }
}