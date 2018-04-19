using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Rx.Tests
{
    public class ObservableCancellationUnitTests
    {
        public CancellationToken TestCancelled => CancellationToken.None;

        [Fact]
        public void TakeUntilShallCancelOperationWhenGivenCancellationTokenToObservable()
        {
            var seen = new List<int>();
            using (var cts = new CancellationTokenSource())
            {
                Enumerable.Range(0, 3).ToObservable().Do(x =>
                    {
                        if (x >= 2)
                            // ReSharper disable once AccessToDisposedClosure -- awaited
                            cts.Cancel();
                    })
                    .TakeUntil(cts.Token.ToObservable())
                    .Do(seen.Add)
                    .Awaiting(o => o.ToTask(TestCancelled))
                    .Should().Throw<OperationCanceledException>();
            }

            seen.Should().BeEquivalentTo(new[] {0, 1});
        }

        [Fact]
        public async Task TakeUntilShallIgnoreWhenCancellationTokenIsNotCancelledIncludingCleaningUp()
        {
            var recordFinallyOnCancel = 0;
            Task<IList<int>> record;
            using (var cts = new CancellationTokenSource())
            {
                var s1 = new Subject<int>();
                record = s1.TakeUntil(
                    cts.Token.ToObservable().Finally(() => Interlocked.Increment(ref recordFinallyOnCancel))
                ).ToList().ToTask(TestCancelled);
                s1.OnNext(1);
                s1.OnCompleted();
            }

            (await record.ConfigureAwait(false)).Should().BeEquivalentTo(1);
            recordFinallyOnCancel.Should().Be(1);
        }
    }
}