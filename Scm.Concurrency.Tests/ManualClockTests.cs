using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Concurrency.Tests
{
    public class ManualClockTests
    {
        public CancellationToken CancellationToken => default(CancellationToken);
        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        public async Task ManualClockShouldSupportMultiplePendingOperationsAndSynchroneousOperations(int concurCount)
        {
            var clock = new ManualClock();
            var step = TimeSpan.FromSeconds(1);
            Skip.If(concurCount < 4);
            var concurrent = Enumerable.Range(0, concurCount)
                .Select(i =>
                {
                    if (i % 2 == 0) // Do async
                        return new
                        {
                            i,
                            Task = clock.Delay(i <= 1 ? TimeSpan.Zero : step, CancellationToken).OnCompletion(i)
                        };
                    // Do blocking in separate thread
                    var tcs = new TaskCompletionSource<int>();
                    var thread = new Thread(() =>
                    {
                        clock.Sleep(i <= 1 ? TimeSpan.Zero : step);
                        tcs.SetResult(i);
                    });
                    thread.Start();
                    return
                        new
                        {
                            i,
                            tcs.Task
                        };
                })
                .ToDictionary(x => x.i, x => x.Task);

            // Only 0 should be able to run
            await Task.WhenAll(concurrent.Where(x => x.Key <= 1).Select(x => x.Value)).ConfigureAwait(false);
            concurrent.Where(x => x.Key > 1).Should().OnlyContain(x => !x.Value.IsCompleted);
            clock.PendingCount().Should().BeGreaterOrEqualTo(2).And.BeLessOrEqualTo(concurCount - 2);
            await Task.WhenAll(clock.Advance(step)).ConfigureAwait(false);
            clock.PendingCount().Should().Be(0);
            var done = await Task.WhenAll(concurrent.Values).ConfigureAwait(false);
            done.Should().BeEquivalentTo(concurrent.Keys);
        }
    }
}
