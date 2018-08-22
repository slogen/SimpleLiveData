using System;
using System.Reactive.Subjects;
using FluentAssertions;
using Scm.Rx;
using Xunit;

namespace Scm.Sys.Tests
{
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

            // ReSharper disable once ImplicitlyCapturedClosure -- acceptable
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
