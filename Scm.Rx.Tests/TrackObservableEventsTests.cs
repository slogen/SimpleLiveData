using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Rx.Tests
{
    public class TrackObservableEventsTests
    {
        public CancellationToken CancellationToken => default(CancellationToken);
        [Fact]
        public async Task TracksNextAndCompletion()
        {
            var s = new Subject<int>();
            var te = s.TrackEvents();
            var t = te.ToListAsync(CancellationToken);
            var e = te.Events;
            e.Should().BeEmpty();
            s.OnNext(1);
            e.Should().BeEquivalentTo(Notification.CreateOnNext(1));
            s.OnCompleted();
            e.Should().BeEquivalentTo(Notification.CreateOnNext(1), Notification.CreateOnCompleted<int>());
            var got = await t.ConfigureAwait(false);
            got.Should().BeEquivalentTo(1);
        }
        [Fact]
        public void TrackError()
        {
            var s = new Subject<int>();
            var te = s.TrackEvents();
            var t = te.ToListAsync(CancellationToken);
            var e = te.Events;
            e.Should().BeEmpty();
            s.OnNext(1);
            e.Should().BeEquivalentTo(Notification.CreateOnNext(1));
            var ex = new Exception();
            s.OnError(ex);
            e.Should().BeEquivalentTo(Notification.CreateOnNext(1), Notification.CreateOnError<int>(ex));
            t.Awaiting(async tsk => await tsk.ConfigureAwait(false))
                .Should().Throw<AggregateException>()
                .WithInnerException<Exception>().Which.Should().BeSameAs(ex);
        }
    }
}
