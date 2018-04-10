using System;
using System.Collections.Generic;
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
            Func<Task<IList<int>>> act = async () =>
            {
                using (var cts = new CancellationTokenSource())
                {
                    var s1 = new Subject<int>();
                    var record = s1.TakeUntil(cts.Token.ToObservable()).ToList().ToTask(TestCancelled);
                    s1.OnNext(1);
                    s1.OnNext(2);
                    cts.Cancel();
                    s1.OnNext(3);
                    return await record.ConfigureAwait(false);
                }
            };
            act.Should().Throw<OperationCanceledException>();
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