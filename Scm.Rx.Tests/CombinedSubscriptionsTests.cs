using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Scm.Rx.Tests
{
    public class CombinedSubscriptionsTests
    {
        private class NoComparer: IEqualityComparer<long>
        {
            public bool Equals(long x, long y) => true;
            public int GetHashCode(long obj) => 0;
        }
        [Fact]
        public void CombineShouldUseKeyComparer()
        {
            Func<long, IObservable<int>> sourceFactory = x => Observable.Range(0, 3);
            long Aggregate(long acc, long next) => Math.Max(acc, next);
            var keyComparer = new NoComparer();
            sourceFactory.CombinedSubscriptions(Aggregate, default(long), keyComparer);

        }

        [Fact]
        public void CombineShouldActuallyPerformCombination()
        {
            var sub = new Subject<int>();
            var subCount = 0;
            var unsubCount = 0;
            var source = Observable.Defer(() =>
            {
                Interlocked.Increment(ref subCount);
                return sub.AsObservable().Finally(() => { Interlocked.Increment(ref unsubCount); });
            });
            Func<long, IObservable<int>> sourceFactory = x => source.Where(i => i <= x);
            long Aggregate(long acc, long next) => Math.Max(acc, next);
            var c = sourceFactory.CombinedSubscriptions(Aggregate, -1L);
            var c0 = c(0);
            var l0 = new List<int>();
            var c3 = c(3);
            var l3 = new List<int>();
            subCount.Should().Be(0);
            unsubCount.Should().Be(0);
            using (c0.Subscribe(x => l0.Add(x)))
            {
                subCount.Should().Be(1);
                unsubCount.Should().Be(0);
                sub.OnNext(0);
                l0.Should().BeEquivalentTo(new[] {0});
                sub.OnNext(1);
                l0.Should().BeEquivalentTo(new[] { 0 });
                using (c3.Subscribe(x => l3.Add(x)))
                {
                    subCount.Should().Be(2);
                    unsubCount.Should().Be(1);
                    sub.OnNext(3);
                    l0.Should().BeEquivalentTo(new[] { 0, 3 });
                    l3.Should().BeEquivalentTo(new[] { 3 });
                    sub.OnNext(4);
                    l0.Should().BeEquivalentTo(new[] { 0, 3 });
                    l3.Should().BeEquivalentTo(new[] { 3 });
                    // Resubscribing with same key should have no effect
                    using (c3.Subscribe(x => { }))
                    {
                        subCount.Should().Be(2);
                        unsubCount.Should().Be(1);
                        // Resubscribing with another key to the same aggregate
                        using (c0.Subscribe(x => { }))
                        {
                            subCount.Should().Be(2);
                            unsubCount.Should().Be(1);
                        }
                    }
                    subCount.Should().Be(2);
                    unsubCount.Should().Be(1);
                }
                subCount.Should().Be(3);
                unsubCount.Should().Be(2);
                sub.OnNext(3);
                l0.Should().BeEquivalentTo(new[] { 0, 3 });
                l3.Should().BeEquivalentTo(new[] { 3 });
            }
            subCount.Should().Be(3);
            unsubCount.Should().Be(3);
        }
    }
}
