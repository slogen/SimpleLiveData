using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace Scm.Rx.Tests
{
    public class ObservableBufferExtensionsTests
    {
        protected TimeSpan TickSpd = TimeSpan.FromSeconds(1);

        [Fact]
        public void BufferWithCountOnlyShouldLimitOnCount()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() =>
                    Observable.Interval(TickSpd, sch).Buffer(null, 3, sch),
                0, 0, TickSpd.Multiply(7).Ticks);
            result.Messages.Select(x => x.Value.Value).Should().BeEquivalentTo(
                new[] {0L, 1L, 2L},
                new[] {3L, 4L, 5L});
            // ,new []{ 6L} -- // We will not get the last value as we unsub after 7
        }

        [Fact]
        public void BufferWithNoArgsShouldReturnListIfCompleted()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() =>
                    Observable.Interval(TickSpd, sch).Take(6).Buffer(null, null, sch),
                0, 0, TickSpd.Multiply(7).Ticks);
            result.Messages.Should()
                .HaveCount(2)
                .And.Contain(x => x.Value.Kind == NotificationKind.OnCompleted)
                .And.Subject.Single(x => x.Value.HasValue)
                .Value.Value.Should().BeEquivalentTo(new[] {0L, 1, 2, 3, 4, 5});
        }

        [Fact]
        public void BufferWithNoArgsShouldYieldCompletetionIfUnsubscribedBeforeCompletion()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() =>
                    Observable.Interval(TickSpd, sch).Buffer(null, null, sch),
                0, 0, TickSpd.Multiply(7).Ticks);
            result.Messages.Should().BeEmpty();
        }

        [Fact]
        public void BufferWithTimeSpanAndCountShouldLimitOnCount()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() =>
                    Observable.Interval(TickSpd, sch).Buffer(TickSpd.Multiply(4), 3, sch),
                0, 0, TickSpd.Multiply(7).Ticks);
            result.Messages.Select(x => x.Value.Value).Should().BeEquivalentTo(
                new[] {0L, 1L, 2L},
                new[] {3L, 4L, 5L});
            // ,new []{ 6L} -- // We will not get the last value as we unsub after 7
        }

        [Fact]
        public void BufferWithTimeSpanAndCountShouldLimitOnTimeSpan()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() =>
                    Observable.Interval(TickSpd, sch).Buffer(TickSpd.Multiply(2.5), 3, sch),
                0, 0, TickSpd.Multiply(5.5).Ticks);
            result.Messages.Select(x => x.Value.Value).Should().BeEquivalentTo(
                new[] {0L, 1L}
                , new[] {2L, 3L}
                // ,new []{4L} -- // We will not get the last value as we unsub after 5.5
            );
        }

        [Fact]
        public void BufferWithTimeSpanOnlyShouldLimitOnTimeSpan()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() =>
                    Observable.Interval(TickSpd, sch).Buffer(TickSpd.Multiply(2.5), null, sch),
                0, 0, TickSpd.Multiply(5.5).Ticks);
            result.Messages.Select(x => x.Value.Value).Should().BeEquivalentTo(
                new[] {0L, 1L}
                , new[] {2L, 3L}
                // ,new []{4L} -- // We will not get the last value as we unsub after 5.5
            );
        }

        [Fact]
        public void ObservableProducesAsExpectedUnderTestScheduler()
        {
            var sch = new TestScheduler();
            var result = sch.Start(() => Observable.Interval(TickSpd, sch), 0, 0, TickSpd.Multiply(5.5).Ticks);
            result.Messages.Select(x => x.Value.Value).Should().BeEquivalentTo(new[] {0, 1, 2, 3, 4});
        }
    }
}