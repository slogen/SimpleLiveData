using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Scm.Rx
{
    public static class JoinLatestExtensions
    {
        /// <summary>
        /// Joins <paramref name="left"/> and <paramref name="right"/> by matching each on their respective <paramref name="leftKey"/> and <paramref name="rightKey"/> and applying <paramref name="selector"/> when that happens
        /// </summary>
        public static IObservable<TResult> JoinLatest<TLeft, TRight, TKey, TResult>(
            this IObservable<TLeft> left,
            IObservable<TRight> right,
            Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey,
            Func<TLeft, TRight, TResult> selector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            if (keyComparer == null)
                keyComparer = EqualityComparer<TKey>.Default;
            var l = new ConcurrentDictionary<TKey, TLeft>(keyComparer);
            var r = new ConcurrentDictionary<TKey, TRight>(keyComparer);
            // ReSharper disable once ImplicitlyCapturedClosure -- acceptable
            return left.Select(lv =>
                {
                    var k = leftKey(lv);
                    l[k] = lv;
                    var isMatch = r.TryGetValue(k, out var rv);
                    return new { isMatch, m = isMatch ? selector(lv, rv) : default(TResult) };
                })
                // ReSharper disable once ImplicitlyCapturedClosure -- acceptable
                .Merge(right.Select(rv =>
                {
                    var key = rightKey(rv);
                    r[key] = rv;
                    var isMatch = l.TryGetValue(key, out var lv);
                    return new { isMatch, m = isMatch ? selector(lv, rv) : default(TResult) };
                }))
                .Where(x => x.isMatch)
                .Select(x => x.m);
        }

    }
}