using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Xunit;

namespace Scm.Rx.Tests
{
    public class PublishReplayTests
    {
        [Fact]
        public void PublishReplayImplementsExpectedHistory()
        {
            var a = new List<int>();
            var b = new List<int>();
            var c = new List<int>();
            var source = new Subject<int>();
            var m = source.AsObservable().PublishReplay(1);
            source.OnNext(0);
            using (m.Subscribe(x => a.Add(x)))
            {
                source.OnNext(1);
                a.Should().BeEquivalentTo(new[] { 1 });
                using (m.Subscribe(x => b.Add(x)))
                {
                    a.Should().BeEquivalentTo(new[] { 1 });
                    b.Should().BeEquivalentTo(new[] { 1 });
                    source.OnNext(2);
                    a.Should().BeEquivalentTo(new[] { 1, 2 });
                    b.Should().BeEquivalentTo(new[] { 1, 2 });
                    using (m.Subscribe(x => c.Add(x)))
                    {
                        a.Should().BeEquivalentTo(new[] { 1, 2 });
                        b.Should().BeEquivalentTo(new[] { 1, 2 });
                        c.Should().BeEquivalentTo(new[] { 2 });
                        source.OnNext(3);
                        a.Should().BeEquivalentTo(new[] { 1, 2, 3 });
                        b.Should().BeEquivalentTo(new[] { 1, 2, 3 });
                        c.Should().BeEquivalentTo(new[] { 2, 3 });
                    }

                    source.OnNext(4);
                    a.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
                    b.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
                    c.Should().BeEquivalentTo(new[] { 2, 3 });
                }
            }
        }
    }
}