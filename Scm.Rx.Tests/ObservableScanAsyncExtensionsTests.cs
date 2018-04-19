using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Rx.Tests
{
    public class ObservableScanAsyncExtensionsTests
    {
        protected Task<int> ThrowIfCalled(int acc, int nxt, CancellationToken ct)
        {
            throw new InvalidOperationException();
        }

        [Fact]
        public async Task ScanAsyncOnEmptyReturnsEmpty()
        {
            var result = await new int[0].ToObservable()
                .ScanAsync(1, ThrowIfCalled)
                .ToList();
            result.Should().BeEquivalentTo(new int[0]);
        }

        [Fact]
        public async Task ScanAsyncOnListWillWaitForEachPredecessorBeforeContinuing()
        {
            const int count = 10;
            var delay = TimeSpan.FromMilliseconds(10);
            long raceFor = 0;
            var result = await Enumerable.Range(0, count).ToObservable()
                .ScanAsync(1L, async (acc, nxt, ct) =>
                {
                    raceFor = raceFor + 1;
                    var atEnter = Interlocked.Read(ref raceFor);
                    await Task.Delay(delay, ct).ConfigureAwait(false);
                    var afterDelay = Interlocked.Read(ref raceFor);
                    raceFor = raceFor + 1;
                    return acc + (afterDelay - atEnter); // Is == acc if there is no race
                })
                .ToList();
            result.Should().OnlyContain(x => x == 1)
                .And.HaveCount(count);
        }
    }
}