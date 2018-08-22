using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Xunit;

namespace Scm.Sys.Tests
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
    public class JoinTests
    {
        protected struct Tx
        {
            public char C;
            public int V;
        }
        protected struct Ty
        {
            public char C;
            public int V;
        }

        protected Tx X(int v) => new Tx {C = (char) ('a' + v), V = v};
        protected Ty Y(int v) => new Ty { C = (char)('A' + v), V = v };

        protected struct Match
        {
            public Tx X;
            public Ty Y;
        }

        protected Match M(Tx x, Ty y) => new Match { X = x, Y = y };


        [Fact]
        public void JoinLatestReallyJoinsOnLatestValue()
        {
            var l = new Subject<Tx>();
            var r = new Subject<Ty>();
            var m = default(Match);
            var j = l.JoinLatest(r, x => x.V, y => y.V, M);

            void CheckMDefault()
                => m.Should().Be(M(default(Tx), default(Ty)));
            void CheckM(int i)
            {
                m.Should().Be(M(X(i), Y(i)));
                m = default(Match);
            }

            using (j.Subscribe(x => m = x))
            {
                l.OnNext(X(1));
                CheckMDefault();

                r.OnNext(Y(1));
                CheckM(1);

                l.OnNext(X(2));
                CheckMDefault();

                r.OnNext(Y(2));
                CheckM(2);

                l.OnNext(X(1));
                CheckM(1);

                r.OnNext(Y(1));
                CheckM(1);

                r.OnNext(Y(3));
                CheckMDefault();

                r.OnNext(Y(4));
                CheckMDefault();

                l.OnNext(X(4));
                CheckM(4);

                l.OnNext(X(3));
                CheckM(3);
            }
        }
    }
}
